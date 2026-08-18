// menu-builder.js - Application menu creation and management
const { Menu, dialog, app, shell, BrowserWindow } = require('electron');
const { loadConfig, saveConfig } = require('./config-manager');
const state = require('./window-state');
const { configureExternalLinkHandling, createWindow } = require('./window-utils');
const PrintPreview = require('./print-preview');
const fs = require('fs');
const path = require('path');

/**
 * Restarts the application
 */
function restartApp() {
  app.relaunch();
  app.exit();
}

/**
 * Gets the active webContents, falling back to focused window's webContents
 * @returns {Electron.WebContents|null} The active webContents or null
 */
function getActiveOrFocusedWebContents() {
  // Try to get active tab's webContents
  const activeWebContents = global.getActiveWebContents ? global.getActiveWebContents() : null;
  if (activeWebContents) return activeWebContents;
  
  // Fallback to focused window's webContents
  const focusedWindow = BrowserWindow.getFocusedWindow();
  return focusedWindow ? focusedWindow.webContents : null;
}

/**
 * Creates the application menu bar
 * @param {BrowserWindow} win - Main application window
 * @param {number} port - Server port number
 * @returns {Menu} The constructed menu
 */
function createApplicationMenu(win, port) {
  const isMac = process.platform === 'darwin';
  
  const template = [
    // File menu
    {
      label: 'File',
      submenu: [
        {
          label: 'Change Data Folder...',
          click: async () => {
            const focusedWindow = BrowserWindow.getFocusedWindow();
            const result = await dialog.showOpenDialog(focusedWindow, {
              title: 'Select Data Folder',
              properties: ['openDirectory', 'createDirectory'],
              buttonLabel: 'Select Folder'
            });
            
            if (!result.canceled && result.filePaths.length > 0) {
              const newPath = result.filePaths[0];
              const config = loadConfig();
              config.dataPath = newPath;
              saveConfig(config);
              
              const choice = await dialog.showMessageBox(focusedWindow, {
                type: 'info',
                title: 'Restart Required',
                message: 'The application needs to restart to use the new data folder.',
                buttons: ['Restart Now', 'Cancel'],
                defaultId: 0,
                cancelId: 1
              });
              
              if (choice.response === 0) {
                restartApp();
              } else {
                // Revert the config change if user cancels
                delete config.dataPath;
                saveConfig(config);
              }
            }
          }
        },
        {
          label: 'Reset Data Folder',
          click: async () => {
            const focusedWindow = BrowserWindow.getFocusedWindow();
            const config = loadConfig();
            if (!config.dataPath) {
              dialog.showMessageBox(focusedWindow, {
                type: 'info',
                title: 'Already Using Default',
                message: 'The application is already using the default data folder.',
                buttons: ['OK']
              });
              return;
            }
            
            const choice = await dialog.showMessageBox(focusedWindow, {
              type: 'question',
              title: 'Reset Data Folder',
              message: 'This will reset to the default data folder. The application will need to restart.',
              buttons: ['Reset and Restart', 'Cancel'],
              defaultId: 0,
              cancelId: 1
            });
            
            if (choice.response === 0) {
              delete config.dataPath;
              saveConfig(config);
              restartApp();
            }
          }
        },
        { type: 'separator' },
        {
          label: 'Print',
          accelerator: 'CmdOrCtrl+P',
          click: async () => {
            const webContents = getActiveOrFocusedWebContents();
            const focusedWindow = BrowserWindow.getFocusedWindow();
            
            if (webContents && focusedWindow) {
              try {
                // Create a wrapper object that includes both webContents and the parent window
                // The print preview needs the parent window for modal behavior
                const tabWrapper = { 
                  webContents: webContents,
                  // Pass the actual window as parent for modal dialog
                  parent: focusedWindow
                };
                const printPreview = new PrintPreview(tabWrapper);
                await printPreview.show();
              } catch (error) {
                dialog.showErrorBox('Print Error', `Failed to open print preview: ${error.message}`);
              }
            }
          }
        },
        { type: 'separator' },
        isMac ? {
          label: 'Close',
          accelerator: 'Cmd+W',
          click: () => {
            // Try to close tab first, then window if no tabs
            const focusedWindow = BrowserWindow.getFocusedWindow();
            if (focusedWindow && global.windowManager) {
              const windowData = global.windowManager.windows.get(focusedWindow.id);
              if (windowData && windowData.activeTabId) {
                // Close the active tab
                global.windowManager.closeTab(focusedWindow.id, windowData.activeTabId);
              } else {
                // No tabs, close the window
                focusedWindow.close();
              }
            } else if (focusedWindow) {
              // Fallback to closing window
              focusedWindow.close();
            }
          }
        } : { role: 'quit' }
      ]
    },
    // Edit menu
    {
      label: 'Edit',
      submenu: [
        { role: 'undo' },
        { role: 'redo' },
        { type: 'separator' },
        { role: 'cut' },
        { role: 'copy' },
        { role: 'paste' },
        ...(isMac ? [
          { role: 'pasteAndMatchStyle' },
          { role: 'delete' },
          { role: 'selectAll' },
          { type: 'separator' },
          {
            label: 'Speech',
            submenu: [
              { role: 'startSpeaking' },
              { role: 'stopSpeaking' }
            ]
          }
        ] : [
          { role: 'delete' },
          { type: 'separator' },
          { role: 'selectAll' }
        ])
      ]
    },
    // View menu
    {
      label: 'View',
      submenu: [
        {
          label: 'Back',
          accelerator: 'Alt+Left',
          id: 'back',
          enabled: false,
          click: () => {
            const webContents = getActiveOrFocusedWebContents();
            if (webContents && webContents.navigationHistory.canGoBack()) {
              webContents.goBack();
            }
          }
        },
        {
          label: 'Forward',
          accelerator: 'Alt+Right',
          id: 'forward',
          enabled: false,
          click: () => {
            const webContents = getActiveOrFocusedWebContents();
            if (webContents && webContents.navigationHistory.canGoForward()) {
              webContents.goForward();
            }
          }
        },
        { type: 'separator' },
        {
          label: 'Reload',
          accelerator: 'CmdOrCtrl+R',
          click: () => {
            const webContents = getActiveOrFocusedWebContents();
            if (webContents) {
              webContents.reload();
            }
          }
        },
        {
          label: 'Force Reload',
          accelerator: 'CmdOrCtrl+Shift+R',
          click: () => {
            const webContents = getActiveOrFocusedWebContents();
            if (webContents) {
              webContents.reloadIgnoringCache();
            }
          }
        },
        {
          label: 'Toggle Developer Tools',
          accelerator: isMac ? 'Alt+Cmd+I' : 'Ctrl+Shift+I',
          click: () => {
            const webContents = getActiveOrFocusedWebContents();
            if (webContents) {
              webContents.toggleDevTools();
            }
          }
        },
        { type: 'separator' },
        {
          label: 'New Tab',
          accelerator: 'CmdOrCtrl+T',
          click: () => {
            const { BrowserWindow } = require('electron');
            const focusedWindow = BrowserWindow.getFocusedWindow();
            if (focusedWindow && global.windowManager) {
              const newTab = global.windowManager.createTab(focusedWindow.id, `http://127.0.0.1:${global.backendPort || 55777}`);
              if (newTab) {
                global.windowManager.switchToTab(focusedWindow.id, newTab.id);
              }
            }
          }
        },
        {
          label: 'Close Tab',
          accelerator: 'CmdOrCtrl+W',
          click: () => {
            const { BrowserWindow } = require('electron');
            const focusedWindow = BrowserWindow.getFocusedWindow();
            if (focusedWindow && global.windowManager) {
              const windowData = global.windowManager.windows.get(focusedWindow.id);
              if (windowData && windowData.activeTabId) {
                global.windowManager.closeTab(focusedWindow.id, windowData.activeTabId);
              }
            }
          }
        },
        {
          label: 'Close Tab (Alt)',
          accelerator: process.platform !== 'darwin' ? 'Ctrl+F4' : undefined,
          visible: false, // Hidden menu item for additional shortcut
          click: () => {
            const { BrowserWindow } = require('electron');
            const focusedWindow = BrowserWindow.getFocusedWindow();
            if (focusedWindow && global.windowManager) {
              const windowData = global.windowManager.windows.get(focusedWindow.id);
              if (windowData && windowData.activeTabId) {
                global.windowManager.closeTab(focusedWindow.id, windowData.activeTabId);
              }
            }
          }
        },
        {
          label: 'Duplicate Tab',
          accelerator: 'CmdOrCtrl+Shift+D',
          click: () => {
            const { BrowserWindow } = require('electron');
            const focusedWindow = BrowserWindow.getFocusedWindow();
            if (focusedWindow && global.windowManager) {
              const windowData = global.windowManager.windows.get(focusedWindow.id);
              if (windowData && windowData.activeTabId) {
                global.windowManager.duplicateTab(focusedWindow.id, windowData.activeTabId);
              }
            }
          }
        },
        {
          label: 'Next Tab',
          accelerator: process.platform === 'darwin' ? 'Cmd+Option+Right' : 'Ctrl+Tab',
          click: () => {
            if (global.switchToNextTab) {
              global.switchToNextTab();
            }
          }
        },
        {
          label: 'Next Tab (Alt)',
          accelerator: 'Control+Tab',  // Works on all platforms including macOS
          visible: false,
          click: () => {
            if (global.switchToNextTab) {
              global.switchToNextTab();
            }
          }
        },
        {
          label: 'Previous Tab',
          accelerator: process.platform === 'darwin' ? 'Cmd+Option+Left' : 'Ctrl+Shift+Tab',
          click: () => {
            if (global.switchToPreviousTab) {
              global.switchToPreviousTab();
            }
          }
        },
        {
          label: 'Previous Tab (Alt)',
          accelerator: 'Control+Shift+Tab',  // Works on all platforms including macOS
          visible: false,
          click: () => {
            if (global.switchToPreviousTab) {
              global.switchToPreviousTab();
            }
          }
        },
        {
          label: 'Reopen Closed Tab',
          accelerator: 'CmdOrCtrl+Shift+T',
          click: () => {
            if (global.reopenLastClosedTab) {
              global.reopenLastClosedTab();
            }
          }
        },
        { type: 'separator' },
        // Add shortcuts for switching to specific tabs (Ctrl+1 through Ctrl+9)
        ...Array.from({ length: 9 }, (_, i) => ({
          label: `Go to Tab ${i + 1}`,
          accelerator: `CmdOrCtrl+${i + 1}`,
          visible: false, // Hide these from menu but keep shortcuts active
          click: () => {
            if (global.switchToTabByIndex) {
              global.switchToTabByIndex(i);
            }
          }
        })),
        { role: 'resetZoom' },
        { 
          role: 'zoomIn',
          accelerator: process.platform === 'darwin' ? 'Cmd+Plus' : 'Ctrl+='
        },
        { role: 'zoomOut' },
        { type: 'separator' },
        { role: 'togglefullscreen' }
      ]
    },
    // Window menu
    {
      label: 'Window',
      submenu: [
        {
          label: 'New Window',
          accelerator: 'CmdOrCtrl+N',
          click: () => {
            const winState = state.load();
            const focusedWindow = BrowserWindow.getFocusedWindow();
            let currentURL = `http://127.0.0.1:${port}`; // Default URL
            
            if (focusedWindow && global.getActiveWebContents) {
              // Get the current URL from the active tab
              const activeWebContents = global.getActiveWebContents();
              if (activeWebContents) {
                currentURL = activeWebContents.getURL() || currentURL;
              }
            }
            
            // Use the window manager if available
            if (global.windowManager) {
              global.windowManager.createWindow({
                url: currentURL,
                sourceWindow: focusedWindow,
                state: winState
              });
            } else {
              // Fallback to old method
              createWindow({
                port,
                state: winState,
                sourceWindow: focusedWindow,
                createContextMenu,
                updateNavigationMenu,
                url: currentURL
              });
            }
          }
        },
        { type: 'separator' },
        { role: 'minimize' },
        {
          label: 'Close Window',
          accelerator: process.platform === 'darwin' ? 'Cmd+Shift+W' : 'Alt+F4',
          click: () => {
            const focusedWindow = BrowserWindow.getFocusedWindow();
            if (focusedWindow) {
              focusedWindow.close();
            }
          }
        },
        ...(process.platform === 'darwin' ? [
          { type: 'separator' },
          { role: 'front' },
          { type: 'separator' },
          { role: 'window' }
        ] : [
          { type: 'separator' },
          {
            label: 'Bring All to Front',
            click: () => {
              const windows = BrowserWindow.getAllWindows();
              windows.forEach(window => {
                window.show();
                window.focus();
              });
            }
          }
        ])
      ]
    },
    // Help menu
    {
      role: 'help',
      submenu: [
        {
          label: 'Manager Website',
          click: async () => {
            await shell.openExternal('https://www.manager.io');
          }
        },
        {
          label: 'Guides',
          click: async () => {
            await shell.openExternal('https://www.manager.io/guides');
          }
        },
        {
          label: 'Ask Assistant',
          click: async () => {
            await shell.openExternal('https://www.manager.io/chatbot');
          }
        },
        {
          label: 'Find Accountants',
          click: async () => {
            await shell.openExternal('https://www.manager.io/accountants');
          }
        },
        {
          label: 'Community Forum',
          click: async () => {
            await shell.openExternal('https://forum.manager.io');
          }
        },
        { type: 'separator' },
        {
          label: 'About',
          click: () => {
            const focusedWindow = BrowserWindow.getFocusedWindow();
            dialog.showMessageBox(focusedWindow, {
              type: 'info',
              title: 'About Manager',
              message: 'Manager.io',
              detail: 'Free Accounting Software\n\nVersion: ' + app.getVersion(),
              buttons: ['OK']
            });
          }
        }
      ]
    }
  ];

  // macOS specific menu adjustments
  if (isMac) {
    template.unshift({
      label: app.getName(),
      submenu: [
        { role: 'about' },
        { type: 'separator' },
        { role: 'services', submenu: [] },
        { type: 'separator' },
        { role: 'hide' },
        { role: 'hideOthers' },
        { role: 'unhide' },
        { type: 'separator' },
        { role: 'quit' }
      ]
    });
  }

  return Menu.buildFromTemplate(template);
}

/**
 * Creates a context menu for text editing and links
 * @param {Object} params - Context menu parameters
 * @param {number} port - Server port number
 * @param {Electron.WebContents} webContents - The webContents where the context menu was triggered
 * @returns {Menu} The context menu
 */
function createContextMenu(params, port, webContents) {
  const template = [];
  
  // If right-clicked on a link, add both "Open in New Tab" and "Open in New Window" options FIRST
  if (params && params.linkURL) {
    // Open in New Tab option - FIRST ITEM
    template.push({
      label: 'Open Link in New Tab',
      click: () => {
        // Check if we have tab support (global function from main.js)
        if (global.createTab) {
          // Create the tab but don't switch to it - stay on current tab
          global.createTab(params.linkURL);
        } else {
          // Fallback: open in new window if tabs aren't available
          const winState = state.load();
          const focusedWindow = BrowserWindow.getFocusedWindow();
          
          // Use the window manager if available
          if (global.windowManager) {
            global.windowManager.createWindow({
              url: params.linkURL,
              sourceWindow: focusedWindow,
              state: winState
            });
          } else {
            createWindow({
              port,
              state: winState,
              sourceWindow: focusedWindow,
              createContextMenu,
              updateNavigationMenu,
              url: params.linkURL
            });
          }
        }
      }
    });
    
    // Open in New Window option
    template.push({
      label: 'Open Link in New Window',
      click: () => {
        const winState = state.load();
        const focusedWindow = BrowserWindow.getFocusedWindow();
        
        // Use the window manager if available
        if (global.windowManager) {
          global.windowManager.createWindow({
            url: params.linkURL,
            sourceWindow: focusedWindow,
            state: winState
          });
        } else {
          // Fallback to old method
          createWindow({
            port,
            state: winState,
            sourceWindow: focusedWindow,
            createContextMenu,
            updateNavigationMenu,
            url: params.linkURL
          });
        }
      }
    });
    
    // Copy Link option
    template.push({
      label: 'Copy Link',
      click: () => {
        const { clipboard } = require('electron');
        clipboard.writeText(params.linkURL);
      }
    });
    
    template.push({ type: 'separator' });
  }
  
  // Add navigation options after link options
  // Use the provided webContents or fall back to active/focused
  if (!webContents) {
    webContents = getActiveOrFocusedWebContents();
  }
  
  if (webContents) {
    template.push({
      label: 'Back',
      enabled: webContents.navigationHistory.canGoBack(),
      click: () => {
        if (webContents.navigationHistory.canGoBack()) {
          webContents.goBack();
        }
      }
    });
    
    template.push({
      label: 'Forward',
      enabled: webContents.navigationHistory.canGoForward(),
      click: () => {
        if (webContents.navigationHistory.canGoForward()) {
          webContents.goForward();
        }
      }
    });
    
    template.push({
      label: 'Reload',
      click: () => {
        webContents.reload();
      }
    });
    
    template.push({ type: 'separator' });
  }
  
  // Add standard text editing options
  template.push(
    { role: 'cut' },
    { role: 'copy' },
    { role: 'paste' },
    { type: 'separator' },
    { role: 'selectAll' }
  );
  
  return Menu.buildFromTemplate(template);
}

/**
 * Updates the navigation menu items (Back/Forward) based on current state
 * @param {BrowserWindow} window - The window to check navigation state for
 */
function updateNavigationMenu(window) {
  if (!window || !window.webContents) return;
  
  const menu = Menu.getApplicationMenu();
  if (!menu) return;
  
  const backItem = menu.getMenuItemById('back');
  const forwardItem = menu.getMenuItemById('forward');
  
  if (backItem) {
    backItem.enabled = window.webContents.navigationHistory.canGoBack();
  }
  
  if (forwardItem) {
    forwardItem.enabled = window.webContents.navigationHistory.canGoForward();
  }
}

module.exports = {
  createApplicationMenu,
  createContextMenu,
  restartApp,
  updateNavigationMenu
};