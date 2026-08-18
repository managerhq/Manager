// window-manager.js - Manages multiple windows with tabs
const { BrowserWindow, BrowserView, WebContentsView, Menu, ipcMain } = require('electron');
const path = require('path');
const { configureExternalLinkHandling } = require('./window-utils');
const { createContextMenu, updateNavigationMenu } = require('./menu-builder');
const { parseTabTitle } = require('./tab-utils');
const windowState = require('./window-state');

class WindowManager {
  constructor() {
    this.windows = new Map(); // Map of window ID to window data
    this.port = 0;
    this.persistedUISettings = null; // Will store persisted RTL/dark mode settings
  }

  setPort(port) {
    this.port = port;
  }
  
  setPersistedUISettings(settings) {
    this.persistedUISettings = settings;
  }
  
  cleanup() {
    // Clean up all windows and their resources
    for (const [windowId, windowData] of this.windows) {
      try {
        // Remove all tabs
        if (windowData.tabs) {
          windowData.tabs.forEach(tab => {
            if (tab.webContents && !tab.webContents.isDestroyed()) {
              tab.webContents.destroy();
            }
          });
        }
        
        // Close tab bar
        if (windowData.tabBar && !windowData.tabBar.isDestroyed()) {
          windowData.tabBar.destroy();
        }
        
        // Note: Don't close the window here as it might already be closing
      } catch (err) {
        console.error(`Error cleaning up window ${windowId}:`, err);
      }
    }
    
    // Clear the windows map
    this.windows.clear();
  }

  createWindow(options = {}) {
    const { url, sourceWindow, state } = options;
    
    // Calculate window position with offset from source window or use saved state
    let x, y;
    if (sourceWindow) {
      const bounds = sourceWindow.getBounds();
      x = bounds.x + 30;
      y = bounds.y + 30;
    } else if (state?.x !== undefined && state?.y !== undefined) {
      // Use saved position from state
      x = state.x;
      y = state.y;
    }
    
    // Create main application window
    const win = new BrowserWindow({ 
      x,
      y,
      width: state?.width || 1200, 
      height: state?.height || 800, 
      autoHideMenuBar: false,
      titleBarStyle: 'default',      
      webPreferences: {
        contextIsolation: true,
        nodeIntegration: false
      }
    });
    
    // Create tab bar as a BrowserView
    const tabBarHeight = 44;
    const tabBar = new BrowserView({
      webPreferences: {
        preload: path.join(__dirname, 'tab-bar-preload.js'),
        contextIsolation: true,
        nodeIntegration: false
      }
    });
    
    // Set initial background color based on persisted dark mode setting
    const tabBarBackgroundColor = (this.persistedUISettings && this.persistedUISettings.isDark) 
      ? '#44403c'  // Dark mode background
      : '#e5e5e5'; // Light mode background
    tabBar.setBackgroundColor(tabBarBackgroundColor);
    
    win.addBrowserView(tabBar);
    const bounds = win.getContentBounds();
    tabBar.setBounds({ x: 0, y: 0, width: bounds.width, height: tabBarHeight });
    tabBar.webContents.loadFile('tab-bar.html');
    
    // Store window data
    const windowData = {
      window: win,
      tabBar: tabBar,
      tabs: [],
      activeTabId: null,
      recentlyClosedTabs: [],
      saveStateTimeout: null  // For debouncing state saves
    };
    
    this.windows.set(win.id, windowData);
    
    // Function to save window state with debouncing
    const saveWindowState = () => {
      // Clear existing timeout
      if (windowData.saveStateTimeout) {
        clearTimeout(windowData.saveStateTimeout);
      }
      
      // Debounce the save operation (wait 500ms after last change)
      windowData.saveStateTimeout = setTimeout(() => {
        const bounds = win.getBounds();
        windowState.save(bounds);
      }, 500);
    };
    
    // Handle window resize
    win.on('resize', () => {
      const bounds = win.getContentBounds();
      tabBar.setBounds({ x: 0, y: 0, width: bounds.width, height: tabBarHeight });
      
      // Resize all tab views
      windowData.tabs.forEach(tab => {
        tab.view.setBounds({ 
          x: 0, 
          y: tabBarHeight, 
          width: bounds.width, 
          height: bounds.height - tabBarHeight 
        });
      });
      
      // Save window state when resized
      saveWindowState();
    });
    
    // Handle window move
    win.on('move', () => {
      // Save window state when moved
      saveWindowState();
    });
    
    // Save state before window closes
    win.on('close', () => {
      // Clear any pending save timeout
      if (windowData.saveStateTimeout) {
        clearTimeout(windowData.saveStateTimeout);
      }
      
      // Save final window state
      const bounds = win.getBounds();
      windowState.save(bounds);
    });
    
    // Handle window close
    win.on('closed', () => {
      this.windows.delete(win.id);
    });
    
    // Update navigation menu when window gains focus
    win.on('focus', () => {
      if (windowData.activeTabId) {
        const activeTab = windowData.tabs.find(t => t.id === windowData.activeTabId);
        if (activeTab) {
          updateNavigationMenu({ webContents: activeTab.webContents });
        }
      }
    });
    
    // Create the first tab with the application
    const firstTab = this.createTab(win.id, url || `http://127.0.0.1:${this.port}`);
    this.switchToTab(win.id, firstTab.id);
    
    return win;
  }

  createTab(windowId, url) {
    const windowData = this.windows.get(windowId);
    if (!windowData) return null;
    
    const { window: win, tabs } = windowData;
    const tabId = Date.now().toString();
    const tabView = new WebContentsView({
      webPreferences: {
        preload: path.join(__dirname, 'preload.js'),
        contextIsolation: true,  // Always enabled for security
        nodeIntegration: false   // Always disabled for security
      }
    });
    
    // Set initial background color based on persisted dark mode or inherited settings
    let backgroundColor = '#e5e5e5'; // Default light background
    if (tabs.length === 0 && this.persistedUISettings && this.persistedUISettings.isDark) {
      backgroundColor = '#44403c'; // Dark background
    } else if (tabs.length > 0) {
      // Check if we're inheriting dark mode from active tab
      const activeTab = tabs.find(t => t.id === windowData.activeTabId);
      if (activeTab && activeTab.isDark) {
        backgroundColor = '#44403c'; // Dark background
      }
    }
    tabView.setBackgroundColor(backgroundColor);
    
    // Configure tab content area bounds
    const bounds = win.getContentBounds();
    const tabBarHeight = 44;
    tabView.setBounds({ 
      x: 0, 
      y: tabBarHeight, 
      width: bounds.width, 
      height: bounds.height - tabBarHeight 
    });
    
    // Configure external link handling
    configureExternalLinkHandling({ webContents: tabView.webContents }, this.port);
    
    // Set up context menu
    tabView.webContents.on('context-menu', (e, params) => {
      const contextMenu = createContextMenu(params, this.port, tabView.webContents);
      // Position menu at cursor location (top-left corner at cursor)
      contextMenu.popup({ window: win, x: params.x, y: params.y, positioningItem: 0 });
    });
    
    // Helper function to update navigation menu if this is the active tab
    const updateIfActive = (callback) => {
      if (windowData.activeTabId === tabId) {
        callback();
      }
    };
    
    // Update navigation menu when tab content changes
    tabView.webContents.on('did-navigate', () => {
      updateIfActive(() => {
        updateNavigationMenu({ webContents: tabView.webContents });
        this.updateNavigationState(windowId);
      });
    });
    
    tabView.webContents.on('did-navigate-in-page', () => {
      updateIfActive(() => {
        updateNavigationMenu({ webContents: tabView.webContents });
        this.updateNavigationState(windowId);
      });
    });
    
    tabView.webContents.on('did-start-loading', () => {
      updateIfActive(() => {
        updateNavigationMenu({ webContents: tabView.webContents });
        this.updateTabBar(windowId);
      });
    });
    
    tabView.webContents.on('did-stop-loading', () => {
      updateIfActive(() => {
        updateNavigationMenu({ webContents: tabView.webContents });
        this.updateTabBar(windowId);
      });
    });
    
    // Update tab title when page title changes
    tabView.webContents.on('page-title-updated', () => {
      this.updateTabBar(windowId);
    });
    
    // Load the URL
    if (url.startsWith('file://')) {
      tabView.webContents.loadFile(url.replace('file://', ''));
    } else {
      tabView.webContents.loadURL(url);
    }
    
    // Determine RTL and dark mode settings
    let inheritedRTL, inheritedDark;
    
    if (tabs.length === 0 && this.persistedUISettings) {
      // This is the first tab, use persisted settings
      inheritedRTL = this.persistedUISettings.isRTL || false;
      inheritedDark = this.persistedUISettings.isDark || false;
    } else {
      // Get settings from the current active tab
      const activeTab = tabs.find(t => t.id === windowData.activeTabId);
      inheritedRTL = activeTab ? (activeTab.isRTL || false) : false;
      inheritedDark = activeTab ? (activeTab.isDark || false) : false;
    }
    
    const tab = {
      id: tabId,
      view: tabView,
      webContents: tabView.webContents,
      isRTL: inheritedRTL,  // Inherit RTL from active tab
      isDark: inheritedDark  // Inherit dark mode from active tab
    };
    
    tabs.push(tab);
    
    return tab;
  }

  switchToTab(windowId, tabId) {
    const windowData = this.windows.get(windowId);
    if (!windowData) return;
    
    const { window: win, tabs, activeTabId } = windowData;
    const tab = tabs.find(t => t.id === tabId);
    if (!tab) return;
    
    // Remove current tab if there is one
    if (activeTabId) {
      const currentTab = tabs.find(t => t.id === activeTabId);
      if (currentTab) {
        win.contentView.removeChildView(currentTab.view);
      }
    }
    
    // Show new active tab
    win.contentView.addChildView(tab.view);
    windowData.activeTabId = tabId;
    
    // Update navigation menu
    updateNavigationMenu({ webContents: tab.webContents });
    this.updateTabBar(windowId);
    this.updateNavigationState(windowId);
  }

  closeTab(windowId, tabId) {
    const windowData = this.windows.get(windowId);
    if (!windowData) return;
    
    const { window: win, tabs, activeTabId, recentlyClosedTabs } = windowData;
    const tabIndex = tabs.findIndex(t => t.id === tabId);
    if (tabIndex === -1) return;
    
    const tab = tabs[tabIndex];
    
    // Store the URL for reopening
    const url = tab.webContents.getURL();
    if (url && !url.startsWith('data:')) {
      recentlyClosedTabs.unshift(url);
      if (recentlyClosedTabs.length > 10) {
        recentlyClosedTabs.pop();
      }
    }
    
    // Remove the tab view if it's active
    if (tabId === activeTabId) {
      win.contentView.removeChildView(tab.view);
    }
    
    // Destroy the webContents
    tab.webContents.close();
    
    // Remove from tabs array
    tabs.splice(tabIndex, 1);
    
    // Check if no tabs remain
    if (tabs.length === 0) {
      // No tabs left, close the window
      win.close();
      return;
    }
    
    // If we closed the active tab, switch to another
    if (tabId === activeTabId) {
      // Switch to the next tab or previous if it was the last
      const newIndex = Math.min(tabIndex, tabs.length - 1);
      this.switchToTab(windowId, tabs[newIndex].id);
    }
    
    this.updateTabBar(windowId);
  }

  updateTabBar(windowId) {
    const windowData = this.windows.get(windowId);
    if (!windowData) return;
    
    const { tabBar, tabs, activeTabId } = windowData;
    
    const tabData = tabs.map(tab => {
      const title = tab.webContents.getTitle() || 'New Tab';
      const { businessName, screenName } = parseTabTitle(title);
      
      return {
        id: tab.id,
        title: title, // Full title for tooltip
        businessName: businessName,
        screenName: screenName,
        url: tab.webContents.getURL(),
        isActive: tab.id === activeTabId,
        isLoading: tab.webContents.isLoading(),
        isRTL: tab.isRTL || false
      };
    });
    
    // Get the RTL and dark mode status of the active tab
    const activeTab = tabs.find(t => t.id === activeTabId);
    const isRTL = activeTab ? (activeTab.isRTL || false) : false;
    const isDark = activeTab ? (activeTab.isDark || false) : false;
    
    tabBar.webContents.send('update-tabs', tabData, isRTL, isDark);
    
    // Update navigation state
    this.updateNavigationState(windowId);
  }
  
  updateNavigationState(windowId) {
    const windowData = this.windows.get(windowId);
    if (!windowData) return;
    
    const { tabBar, tabs, activeTabId } = windowData;
    const activeTab = tabs.find(t => t.id === activeTabId);
    
    if (activeTab && activeTab.webContents) {
      const canGoBack = activeTab.webContents.navigationHistory.canGoBack();
      const canGoForward = activeTab.webContents.navigationHistory.canGoForward();
      tabBar.webContents.send('navigation-state-update', canGoBack, canGoForward);
    } else {
      tabBar.webContents.send('navigation-state-update', false, false);
    }
  }
  
  updateTabRTLStatus(windowId, webContents, isRTL) {
    const windowData = this.windows.get(windowId);
    if (!windowData) return;
    
    const { tabs } = windowData;
    const tab = tabs.find(t => t.webContents === webContents);
    if (tab) {
      tab.isRTL = isRTL;
      this.updateTabBar(windowId);
    }
  }
  
  updateTabDarkModeStatus(windowId, webContents, isDark) {
    const windowData = this.windows.get(windowId);
    if (!windowData) return;
    
    const { tabs } = windowData;
    const tab = tabs.find(t => t.webContents === webContents);
    if (tab) {
      tab.isDark = isDark;
      this.updateTabBar(windowId);
    }
  }

  // Get window ID from event sender
  getWindowIdFromEvent(event) {
    const windows = BrowserWindow.getAllWindows();
    for (const win of windows) {
      if (win.webContents === event.sender) {
        return win.id;
      }
      // Check if sender is from tab bar
      for (const [winId, data] of this.windows.entries()) {
        if (data.tabBar && data.tabBar.webContents === event.sender) {
          return winId;
        }
        // Check if sender is from a tab
        for (const tab of data.tabs) {
          if (tab.webContents === event.sender) {
            return winId;
          }
        }
      }
    }
    return null;
  }

  // Helper methods for tab navigation
  switchToNextTab(windowId) {
    const windowData = this.windows.get(windowId);
    if (!windowData || windowData.tabs.length <= 1) return;
    
    const currentIndex = windowData.tabs.findIndex(t => t.id === windowData.activeTabId);
    const nextIndex = (currentIndex + 1) % windowData.tabs.length;
    this.switchToTab(windowId, windowData.tabs[nextIndex].id);
  }

  switchToPreviousTab(windowId) {
    const windowData = this.windows.get(windowId);
    if (!windowData || windowData.tabs.length <= 1) return;
    
    const currentIndex = windowData.tabs.findIndex(t => t.id === windowData.activeTabId);
    const previousIndex = (currentIndex - 1 + windowData.tabs.length) % windowData.tabs.length;
    this.switchToTab(windowId, windowData.tabs[previousIndex].id);
  }

  switchToTabByIndex(windowId, index) {
    const windowData = this.windows.get(windowId);
    if (!windowData) return;
    
    // Ctrl+9 goes to the last tab
    if (index === 8 && windowData.tabs.length > 9) {
      this.switchToTab(windowId, windowData.tabs[windowData.tabs.length - 1].id);
    } else if (index < windowData.tabs.length) {
      this.switchToTab(windowId, windowData.tabs[index].id);
    }
  }

  reopenLastClosedTab(windowId) {
    const windowData = this.windows.get(windowId);
    if (!windowData || windowData.recentlyClosedTabs.length === 0) return;
    
    const url = windowData.recentlyClosedTabs.shift();
    const newTab = this.createTab(windowId, url);
    this.switchToTab(windowId, newTab.id);
  }

  duplicateTab(windowId, tabId) {
    const windowData = this.windows.get(windowId);
    if (!windowData) return;
    
    const { tabs } = windowData;
    const tabToDuplicate = tabs.find(t => t.id === tabId);
    if (!tabToDuplicate) return;
    
    // Don't duplicate the overview tab
    if (tabToDuplicate.isOverviewTab) return;
    
    // Get the URL of the tab to duplicate
    const url = tabToDuplicate.webContents.getURL();
    
    // Create a new tab with the same URL
    const newTab = this.createTab(windowId, url);
    if (newTab) {
      // Move the new tab to be right after the duplicated tab
      const originalIndex = tabs.findIndex(t => t.id === tabId);
      const newTabIndex = tabs.findIndex(t => t.id === newTab.id);
      
      if (originalIndex !== -1 && newTabIndex !== -1) {
        // Remove the new tab from its current position
        const [movedTab] = tabs.splice(newTabIndex, 1);
        // Insert it right after the original tab
        tabs.splice(originalIndex + 1, 0, movedTab);
      }
      
      // Switch to the new duplicated tab
      this.switchToTab(windowId, newTab.id);
    }
    
    return newTab;
  }
}

module.exports = WindowManager;