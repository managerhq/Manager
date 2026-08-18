// print-preview-preload.js - Secure preload script for print preview window
const { contextBridge, ipcRenderer, shell } = require('electron');

// Expose only necessary APIs through contextBridge for security
contextBridge.exposeInMainWorld('electronAPI', {
  // IPC communication for print preview updates
  sendPrintPreviewUpdate: (settings) => {
    ipcRenderer.send('print-preview-update', settings);
  },
  
  // Listen for PDF ready events
  onPrintPreviewPdfReady: (callback) => {
    ipcRenderer.on('print-preview-pdf-ready', () => {
      callback();
    });
  },
  
  // Remove listener
  removePrintPreviewPdfReadyListener: () => {
    ipcRenderer.removeAllListeners('print-preview-pdf-ready');
  },
  
  // Safe external link handling
  openExternal: (url) => {
    // Validate URL before opening
    try {
      const urlObj = new URL(url);
      // Only allow http and https protocols
      if (urlObj.protocol === 'http:' || urlObj.protocol === 'https:') {
        shell.openExternal(url);
      }
    } catch (e) {
      // Invalid URL, don't open
      console.error('Invalid URL:', url);
    }
  }
});