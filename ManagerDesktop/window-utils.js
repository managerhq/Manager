// window-utils.js - Window management utilities
const { shell, BrowserWindow } = require('electron');
const path = require('path');
const { isExternalUrl } = require('./tab-utils');

/**
 * Configures a BrowserWindow to handle external links properly
 * @param {BrowserWindow} window - The window to configure
 * @param {number} port - Server port number
 */
function configureExternalLinkHandling(window, port) {
  // Handle external link navigation
  window.webContents.on('will-navigate', (event, url) => {
    try {
      // Check if the URL is external (not localhost)
      if (isExternalUrl(url, port)) {
        event.preventDefault();
        shell.openExternal(url);
      }
    } catch (e) {
      console.error('Error in will-navigate handler:', e);
    }
  });
  
  // Handle new window requests (e.g., target="_blank" links)
  // Only set this handler if it's a BrowserWindow (has setWindowOpenHandler)
  if (window.webContents.setWindowOpenHandler) {
    window.webContents.setWindowOpenHandler(({ url }) => {
      try {
        // Check if the URL is external
        if (isExternalUrl(url, port)) {
          shell.openExternal(url);
        }
        // Always deny - we handle navigation through tabs
        return { action: 'deny' };
      } catch (e) {
        console.error('Error in setWindowOpenHandler:', e);
        return { action: 'deny' };
      }
    });
  }
}

/**
 * Creates and configures a new browser window with standard setup
 * @param {Object} options - Window creation options
 * @param {number} options.port - Server port number
 * @param {Object} options.state - Window state (width, height)
 * @param {BrowserWindow} options.sourceWindow - Source window for positioning offset
 * @param {Function} options.createContextMenu - Function to create context menus
 * @param {Function} options.updateNavigationMenu - Function to update navigation menu
 * @param {string} options.url - URL to load in the window
 * @returns {BrowserWindow} The newly created window
 */
function createWindow(options) {
  const { port, state, sourceWindow, createContextMenu, updateNavigationMenu, url } = options;
  
  // Calculate window position with offset from source window
  let x, y;
  if (sourceWindow) {
    const bounds = sourceWindow.getBounds();
    // Offset by 30 pixels to make it obvious it's a new window
    x = bounds.x + 30;
    y = bounds.y + 30;
  }
  
  // Create the new window
  const newWindow = new BrowserWindow({
    x: x,
    y: y,
    width: state.width,
    height: state.height,
    autoHideMenuBar: false,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,  // Enabled for security
      nodeIntegration: false   // Already disabled for security
    }
  });
  
  // Configure context menu
  newWindow.webContents.on('context-menu', (e, params) => {
    const contextMenu = createContextMenu(params, port, newWindow.webContents);
    // Position menu at cursor location (top-left corner at cursor)
    contextMenu.popup({ window: newWindow, x: params.x, y: params.y, positioningItem: 0 });
  });
  
  // Configure navigation event handlers
  newWindow.webContents.on('did-navigate', () => {
    updateNavigationMenu(newWindow);
  });
  
  newWindow.webContents.on('did-navigate-in-page', () => {
    updateNavigationMenu(newWindow);
  });
  
  newWindow.webContents.on('did-start-loading', () => {
    updateNavigationMenu(newWindow);
  });
  
  newWindow.webContents.on('did-stop-loading', () => {
    updateNavigationMenu(newWindow);
  });
  
  // Update navigation menu when window gains focus
  newWindow.on('focus', () => {
    updateNavigationMenu(newWindow);
  });
  
  // Configure external link handling
  configureExternalLinkHandling(newWindow, port);
  
  // Load the URL if provided
  if (url) {
    newWindow.loadURL(url);
  }
  
  return newWindow;
}

module.exports = {
  configureExternalLinkHandling,
  createWindow
};