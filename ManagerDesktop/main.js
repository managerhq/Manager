// main-new.js - Main process entry point with multi-window tab support
const { app, BrowserWindow, Menu, ipcMain, dialog } = require('electron');
const state = require('./window-state');
const WindowManager = require('./window-manager');
const { createApplicationMenu, createContextMenu, updateNavigationMenu } = require('./menu-builder');
const { findPortWithPreference, startBackend, waitForPort } = require('./backend-manager');
const PrintPreview = require('./print-preview');

// Global variables
let backend, port = 0;
const windowManager = new WindowManager();

// Export windowManager globally for menu-builder
global.windowManager = windowManager;

// Export functions for menu-builder to use
global.getActiveWebContents = () => {
  const focusedWindow = BrowserWindow.getFocusedWindow();
  if (!focusedWindow) return null;
  
  const windowData = windowManager.windows.get(focusedWindow.id);
  if (!windowData || !windowData.activeTabId) return null;
  
  const activeTab = windowData.tabs.find(t => t.id === windowData.activeTabId);
  return activeTab ? activeTab.webContents : null;
};

global.getActiveTabId = () => {
  const focusedWindow = BrowserWindow.getFocusedWindow();
  if (!focusedWindow) return null;
  
  const windowData = windowManager.windows.get(focusedWindow.id);
  return windowData ? windowData.activeTabId : null;
};

global.createTab = (url) => {
  const focusedWindow = BrowserWindow.getFocusedWindow();
  if (!focusedWindow) return null;
  
  return windowManager.createTab(focusedWindow.id, url);
};

global.switchToTab = (tabId) => {
  const focusedWindow = BrowserWindow.getFocusedWindow();
  if (!focusedWindow) return;
  
  windowManager.switchToTab(focusedWindow.id, tabId);
};

// Helper function to execute window manager operations on focused window
const withFocusedWindow = (operation) => {
  const focusedWindow = BrowserWindow.getFocusedWindow();
  if (focusedWindow) {
    operation(focusedWindow.id);
  }
};

global.switchToNextTab = () => {
  withFocusedWindow(id => windowManager.switchToNextTab(id));
};

global.switchToPreviousTab = () => {
  withFocusedWindow(id => windowManager.switchToPreviousTab(id));
};

global.switchToTabByIndex = (index) => {
  withFocusedWindow(id => windowManager.switchToTabByIndex(id, index));
};

global.reopenLastClosedTab = () => {
  withFocusedWindow(id => windowManager.reopenLastClosedTab(id));
};

// Ensure only one instance of the app runs at a time
if (!app.requestSingleInstanceLock()) {
  app.quit();
} else {
  // Handle when user tries to open a second instance
  app.on('second-instance', () => {
    const windows = BrowserWindow.getAllWindows();
    if (windows.length > 0) {
      // Focus the first window and add a new tab
      const firstWindow = windows[0];
      firstWindow.focus();
      const newTab = windowManager.createTab(firstWindow.id, `http://127.0.0.1:${port}`);
      if (newTab) {
        windowManager.switchToTab(firstWindow.id, newTab.id);
      }
    } else {
      // Create a new window
      const winState = state.load();
      windowManager.createWindow({
        url: `http://127.0.0.1:${port}`,
        state: winState
      });
    }
  });

  // Main app initialization
  app.whenReady().then(async () => {
    // Find a port (prefer 55667, fallback to random)
    port = await findPortWithPreference(55667);
    windowManager.setPort(port);
    global.backendPort = port;
    
    // Load persisted UI settings and pass to window manager
    const winState = state.load();
    windowManager.setPersistedUISettings({
      isRTL: winState.isRTL,
      isDark: winState.isDark
    });
    
    // Set up the application menu
    const appMenu = createApplicationMenu(null, port);
    Menu.setApplicationMenu(appMenu);
    
    // Set up IPC handlers for print requests
    ipcMain.on('window-print-requested', async (event) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (!windowId) return;
      
      const windowData = windowManager.windows.get(windowId);
      if (!windowData) return;
      
      const requestingTab = windowData.tabs.find(tab => tab.webContents === event.sender);
      
      if (requestingTab && windowData.window) {
        try {
          const tabWrapper = {
            webContents: requestingTab.webContents,
            parent: windowData.window
          };
          const printPreview = new PrintPreview(tabWrapper);
          await printPreview.show();
        } catch (error) {
          console.error('Failed to open print preview:', error);
        }
      }
    });
    
    // Handle iframe print requests
    ipcMain.on('iframe-print-requested', async (event, iframeContent) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (!windowId) return;
      
      const windowData = windowManager.windows.get(windowId);
      if (!windowData) return;
      
      const requestingTab = windowData.tabs.find(tab => tab.webContents === event.sender);
      
      if (requestingTab && windowData.window) {
        try {
          const tabWrapper = {
            webContents: requestingTab.webContents,
            parent: windowData.window,
            iframeContent: iframeContent  // Pass iframe content
          };
          const printPreview = new PrintPreview(tabWrapper);
          await printPreview.show();
        } catch (error) {
          console.error('Failed to open print preview for iframe:', error);
        }
      }
    });
    
    // Set up IPC handler for printToPDF with HTML content
    ipcMain.handle('print-html-to-pdf', async (event, { html, options }) => {
      try {
        if (!html) {
          throw new Error('No HTML content provided');
        }
        
        // Create a hidden window to render the HTML content.
        // javascript:false prevents any <script> / inline handlers in the supplied HTML from executing.
        const hiddenWindow = new BrowserWindow({
          show: false,
          webPreferences: {
            offscreen: true,
            javascript: false
          }
        });
        
        // Load the HTML content
        await hiddenWindow.loadURL(`data:text/html;charset=utf-8,${encodeURIComponent(html)}`);
        
        // Wait for content to be ready
        await new Promise(resolve => setTimeout(resolve, 100));
        
        // Default print options merged with provided options
        const printOptions = {
          marginsType: 0,
          pageSize: 'A4',
          printBackground: true,
          printSelectionOnly: false,
          landscape: false,
          preferCSSPageSize: true,
          ...options // Allow custom options to override defaults
        };
        
        // Generate PDF data
        const pdfData = await hiddenWindow.webContents.printToPDF(printOptions);
        
        // Clean up the hidden window
        hiddenWindow.destroy();
        
        return pdfData;
      } catch (error) {
        console.error('Failed to generate PDF from HTML:', error);
        throw error;
      }
    });

    // Set up IPC handler for opening links in new tabs
    ipcMain.on('open-link-in-new-window', (event, url) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      
      if (windowId) {
        // Create new tab in the same window (background tab)
        windowManager.createTab(windowId, url);
        windowManager.updateTabBar(windowId);
      } else {
        // Create a new window if we can't find the source
        const winState = state.load();
        windowManager.createWindow({
          url,
          state: winState
        });
      }
    });

    // Set up IPC handler for printToPDF
    ipcMain.handle('print-to-pdf', async (event, options) => {
      try {
        const windowId = windowManager.getWindowIdFromEvent(event);
        if (!windowId) {
          throw new Error('Could not find window for print request');
        }
        
        const windowData = windowManager.windows.get(windowId);
        if (!windowData) {
          throw new Error('Could not find window data');
        }
        
        const requestingTab = windowData.tabs.find(tab => tab.webContents === event.sender);
        if (!requestingTab || !requestingTab.webContents) {
          throw new Error('Could not find requesting tab');
        }
        
        // Default print options
        const printOptions = {
          marginsType: 0,
          pageSize: 'A4',
          printBackground: true,
          printSelectionOnly: false,
          landscape: false,
          preferCSSPageSize: true,
          ...options // Allow custom options to override defaults
        };
        
        // Generate PDF data
        const pdfData = await requestingTab.webContents.printToPDF(printOptions);
        return pdfData;
      } catch (error) {
        console.error('Failed to generate PDF:', error);
        throw error;
      }
    });
    
    // IPC handler for RTL status from pages
    ipcMain.on('page-rtl-status', (event, isRTL) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (windowId) {
        windowManager.updateTabRTLStatus(windowId, event.sender, isRTL);
        // Persist the RTL setting
        state.saveUISettings({ isRTL });
      }
    });
    
    // IPC handler for dark mode status from pages
    ipcMain.on('page-dark-mode-status', (event, isDark) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (windowId) {
        windowManager.updateTabDarkModeStatus(windowId, event.sender, isDark);
        // Persist the dark mode setting
        state.saveUISettings({ isDark });
      }
    });
    
    // IPC handlers for tab management
    ipcMain.on('tab-switch', (event, tabId) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (windowId) {
        windowManager.switchToTab(windowId, tabId);
      }
    });
    
    ipcMain.on('tab-close', (event, tabId) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (windowId) {
        windowManager.closeTab(windowId, tabId);
      }
    });
    
    ipcMain.on('tab-new', (event) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (windowId) {
        const newTab = windowManager.createTab(windowId, `http://127.0.0.1:${port}`);
        if (newTab) {
          windowManager.switchToTab(windowId, newTab.id);
        }
      }
    });
    
    // IPC handlers for navigation
    ipcMain.on('navigation-back', (event) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (windowId) {
        const windowData = windowManager.windows.get(windowId);
        if (windowData && windowData.activeTabId) {
          const activeTab = windowData.tabs.find(t => t.id === windowData.activeTabId);
          if (activeTab && activeTab.webContents.navigationHistory.canGoBack()) {
            activeTab.webContents.navigationHistory.goBack();
          }
        }
      }
    });
    
    ipcMain.on('navigation-forward', (event) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (windowId) {
        const windowData = windowManager.windows.get(windowId);
        if (windowData && windowData.activeTabId) {
          const activeTab = windowData.tabs.find(t => t.id === windowData.activeTabId);
          if (activeTab && activeTab.webContents.navigationHistory.canGoForward()) {
            activeTab.webContents.navigationHistory.goForward();
          }
        }
      }
    });
    
    ipcMain.on('navigation-refresh', (event) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (windowId) {
        const windowData = windowManager.windows.get(windowId);
        if (windowData && windowData.activeTabId) {
          const activeTab = windowData.tabs.find(t => t.id === windowData.activeTabId);
          if (activeTab) {
            activeTab.webContents.reload();
          }
        }
      }
    });
    
    // IPC handler for tab context menu
    ipcMain.on('tab-context-menu', (event, tabId) => {
      const windowId = windowManager.getWindowIdFromEvent(event);
      if (!windowId) return;
      
      const windowData = windowManager.windows.get(windowId);
      if (!windowData) return;
      
      const { Menu } = require('electron');
      
      // Check if we're in RTL mode by looking at the active tab
      const activeTab = windowData.tabs.find(t => t.id === windowData.activeTabId);
      const isRTL = activeTab ? (activeTab.isRTL || false) : false;
      
      // Build context menu items
      const menuItems = [];
      
      menuItems.push({
        label: 'Duplicate Tab',
          click: () => {
            windowManager.duplicateTab(windowId, tabId);
          }
        });
        
        menuItems.push({ type: 'separator' });
      
      // Close tab
      menuItems.push({
        label: 'Close Tab',
        click: () => {
          windowManager.closeTab(windowId, tabId);
        }
      });
      
      menuItems.push({
        label: 'Close Other Tabs',
          click: () => {
            const { tabs } = windowData;
            const tabsToClose = tabs.filter(t => t.id !== tabId);
            tabsToClose.forEach(t => {
              windowManager.closeTab(windowId, t.id);
            });
          }
        });
        
        menuItems.push({
          label: isRTL ? 'Close Tabs to the Left' : 'Close Tabs to the Right',
          click: () => {
            const { tabs } = windowData;
            const tabIndex = tabs.findIndex(t => t.id === tabId);
            if (tabIndex !== -1) {
              const tabsToClose = tabs.slice(tabIndex + 1);
              tabsToClose.forEach(t => {
                windowManager.closeTab(windowId, t.id);
              });
            }
          }
        });
      
      // Create and show the context menu
      if (menuItems.length > 0) {
        const contextMenu = Menu.buildFromTemplate(menuItems);
        contextMenu.popup();
      }
    });

    // Start the backend server
    backend = startBackend(port);

    // Monitor backend process for crashes/exits
    backend.on('exit', (code, signal) => {
      console.error('ManagerServer process exited unexpectedly');
      console.error(`Exit code: ${code}, Signal: ${signal}`);
    });

    backend.on('error', (err) => {
      console.error('ManagerServer process error:', err);
    });

    try {
      // Wait for backend server to be ready
      await waitForPort(port);
      
      // Create the first window (winState already loaded above)
      const firstWindow = windowManager.createWindow({
        url: `http://127.0.0.1:${port}`,
        state: winState
      });
      
      // Update menu with actual window reference
      const updatedMenu = createApplicationMenu(firstWindow, port);
      Menu.setApplicationMenu(updatedMenu);
      
    } catch (err) {
      // Show error if backend fails to start
      const winState = state.load();
      const errorWindow = windowManager.createWindow({
        url: `file://error.html?msg=${encodeURIComponent(err.message || 'Unknown error')}`,
        state: winState
      });
    }
  });

  // Cleanup: Kill backend process before app quits
  app.on('before-quit', (event) => {
    try {
      // Clean up window manager
      if (windowManager) {
        windowManager.cleanup();
      }

      // Remove event listeners before intentionally killing the backend
      if (backend) {
        backend.removeAllListeners('exit');
        backend.removeAllListeners('error');

        // Kill backend process
        if (backend.kill) {
          backend.kill();
        }
      }
    } catch (err) {
      console.error('Error during app cleanup:', err);
    }
  });
  
  // Quit app when all windows are closed (except on macOS)
  app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
      app.quit();
    }
  });

  // macOS: Re-create window when dock icon is clicked and no windows exist
  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      const winState = state.load();
      windowManager.createWindow({
        url: `http://127.0.0.1:${port}`,
        state: winState
      });
    }
  });
}