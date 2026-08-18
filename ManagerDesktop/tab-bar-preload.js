// tab-bar-preload.js - Preload script for the tab bar
const { contextBridge, ipcRenderer } = require('electron');

// Expose protected methods that allow the tab bar to interact with the main process
contextBridge.exposeInMainWorld('api', {
  // Switch to a different tab
  switchTab: (tabId) => {
    ipcRenderer.send('tab-switch', tabId);
  },
  
  // Close a tab
  closeTab: (tabId) => {
    ipcRenderer.send('tab-close', tabId);
  },
  
  // Create a new tab
  newTab: () => {
    ipcRenderer.send('tab-new');
  },
  
  // Listen for tab updates from main process
  onUpdateTabs: (callback) => {
    ipcRenderer.on('update-tabs', (event, tabs, isRTL, isDark) => {
      callback(tabs, isRTL, isDark);
    });
  },
  
  // Show context menu for a tab
  showTabContextMenu: (tabId) => {
    ipcRenderer.send('tab-context-menu', tabId);
  },
  
  // Navigation controls
  goBack: () => {
    ipcRenderer.send('navigation-back');
  },
  
  goForward: () => {
    ipcRenderer.send('navigation-forward');
  },
  
  refresh: () => {
    ipcRenderer.send('navigation-refresh');
  },
  
  // Listen for navigation state updates
  onNavigationStateUpdate: (callback) => {
    ipcRenderer.on('navigation-state-update', (event, canGoBack, canGoForward) => {
      callback(canGoBack, canGoForward);
    });
  }
});