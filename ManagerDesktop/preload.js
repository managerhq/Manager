// preload.js - Preload script for intercepting print calls and handling zoom
const { contextBridge, ipcRenderer, webFrame } = require('electron');

// Expose protected APIs to the renderer process
contextBridge.exposeInMainWorld('electronAPI', {
  // Print functionality
  print: () => {
    ipcRenderer.send('window-print-requested');
  },
  
  // Print iframe content
  printIframe: (iframe) => {
    // Get iframe's HTML content
    let iframeContent = null;
    try {
      iframeContent = iframe.contentDocument.documentElement.outerHTML;
    } catch (e) {
      console.error('Cannot access iframe content:', e);
    }
    ipcRenderer.send('iframe-print-requested', iframeContent);
  },
  
  // Print to PDF functionality
  printToPDF: async (options) => {
    try {
      // If HTML content is provided, use dedicated handler for HTML printing
      if (options && options.html) {
        const { html, ...printOptions } = options;
        const pdfData = await ipcRenderer.invoke('print-html-to-pdf', {
          html: html,
          options: printOptions
        });
        return pdfData;
      } else {
        // Otherwise, print current window content
        const pdfData = await ipcRenderer.invoke('print-to-pdf', options);
        return pdfData;
      }
    } catch (error) {
      console.error('Failed to generate PDF:', error);
      throw error;
    }
  },
  
  // Link handling
  openLinkInNewWindow: (url) => {
    if (typeof url === 'string') {
      ipcRenderer.send('open-link-in-new-window', url);
    }
  },
  
  // Zoom functionality
  setZoomLevel: (level) => {
    webFrame.setZoomLevel(level);
  },
  
  getZoomLevel: () => {
    return webFrame.getZoomLevel();
  }
});

// Inject code into the main world (page context) where electronAPI is available
window.addEventListener('DOMContentLoaded', () => {
  // Detect RTL mode and send to main process
  const detectRTL = () => {
    const htmlElement = document.documentElement;
    const isRTL = htmlElement.dir === 'rtl' || htmlElement.getAttribute('dir') === 'rtl';
    ipcRenderer.send('page-rtl-status', isRTL);
  };
  
  // Detect dark mode and send to main process
  const detectDarkMode = () => {
    const htmlElement = document.documentElement;
    const isDark = htmlElement.classList.contains('dark');
    ipcRenderer.send('page-dark-mode-status', isDark);
  };
  
  // Detect RTL on initial load
  detectRTL();
  // Detect dark mode on initial load
  detectDarkMode();
  
  // Watch for changes to the dir attribute and class list
  const observer = new MutationObserver((mutations) => {
    for (const mutation of mutations) {
      if (mutation.type === 'attributes') {
        if (mutation.attributeName === 'dir') {
          detectRTL();
        } else if (mutation.attributeName === 'class') {
          detectDarkMode();
        }
      }
    }
  });
  
  observer.observe(document.documentElement, {
    attributes: true,
    attributeFilter: ['dir', 'class']
  });
  
  // This code will run in the page context where window.electronAPI exists
  const script = document.createElement('script');
  script.textContent = `
    (function() {
      // Override window.print
      window.print = function() {
        window.electronAPI.print();
        return false;
      };
      
      // Function to handle iframes
      function handleIframe(iframe) {
        try {
          if (iframe.contentWindow) {
            iframe.contentWindow.print = function() {
              // Pass iframe identifier to indicate this print request is for iframe content
              window.electronAPI.printIframe(iframe);
              return false;
            };
          }
          
          iframe.addEventListener('load', () => {
            try {
              if (iframe.contentWindow) {
                iframe.contentWindow.print = function() {
                  // Pass iframe identifier to indicate this print request is for iframe content
                  window.electronAPI.printIframe(iframe);
                  return false;
                };
              }
            } catch (e) {
              // Cross-origin iframe, ignore
            }
          });
        } catch (e) {
          // Cross-origin iframe, ignore
        }
      }
      
      // Handle existing iframes
      const iframes = document.querySelectorAll('iframe');
      iframes.forEach(handleIframe);
      
      // Watch for new iframes
      const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
          for (const node of mutation.addedNodes) {
            try {
              if (node.nodeName === 'IFRAME') {
                handleIframe(node);
              }
              if (node.querySelectorAll) {
                const iframes = node.querySelectorAll('iframe');
                iframes.forEach(handleIframe);
              }
            } catch (e) {
              // Silently ignore any errors
            }
          }
        });
      });
      
      observer.observe(document.body, {
        childList: true,
        subtree: true
      });
      
      // Set up link click handlers
      // Handle middle mouse click on links
      document.addEventListener('auxclick', (event) => {
        try {
          if (event.button === 1) {
            const link = event.target.closest('a');
            if (link && link.href) {
              event.preventDefault();
              event.stopPropagation();
              console.log('Middle click detected, opening in new tab:', link.href);
              if (window.electronAPI && window.electronAPI.openLinkInNewWindow) {
                window.electronAPI.openLinkInNewWindow(link.href);
              } else {
                console.error('electronAPI not available');
              }
            }
          }
        } catch (e) {
          console.error('Error handling middle click:', e);
        }
      }, true);

      // Handle Ctrl+Click (or Cmd+Click on Mac) on links
      document.addEventListener('click', (event) => {
        try {
          // Check for Ctrl/Cmd + left click
          if ((event.ctrlKey || event.metaKey) && event.button === 0) {
            const link = event.target.closest('a');
            if (link && link.href) {
              // Prevent all default behaviors
              event.preventDefault();
              event.stopPropagation();
              
              // Open in new tab (same as middle click behavior)
              console.log('Ctrl+Click detected, opening in new tab:', link.href);
              if (window.electronAPI && window.electronAPI.openLinkInNewWindow) {
                window.electronAPI.openLinkInNewWindow(link.href);
              } else {
                console.error('electronAPI not available');
              }
              return false;
            }
          }
        } catch (e) {
          console.error('Error handling Ctrl+Click:', e);
        }
      }, true);

      // Handle Ctrl+MouseWheel for zooming
      document.addEventListener('wheel', (event) => {
        if (event.ctrlKey || event.metaKey) {
          event.preventDefault();
          
          if (window.electronAPI && window.electronAPI.getZoomLevel && window.electronAPI.setZoomLevel) {
            const currentZoom = window.electronAPI.getZoomLevel();
            const zoomDelta = event.deltaY > 0 ? -0.5 : 0.5;
            const newZoom = currentZoom + zoomDelta;
            
            const minZoom = -8;  // ~25% zoom
            const maxZoom = 9;   // ~500% zoom
            
            if (newZoom >= minZoom && newZoom <= maxZoom) {
              window.electronAPI.setZoomLevel(newZoom);
            }
          }
        }
      }, { passive: false });
    })();
  `;
  
  // Inject the script into the page
  document.documentElement.appendChild(script);
  script.remove();
});