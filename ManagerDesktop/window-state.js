// Module for persisting window state (position and size) between app sessions
const path = require('path');
const { app } = require('electron');
const { readJsonFile, writeJsonFile } = require('./json-file-utils');

// Path to the JSON file where window state is saved in user's app data directory
const file = path.join(app.getPath('userData'), 'window-state.json');

/**
 * Loads the saved window state from disk
 * @returns {Object} Window state object with position, size, and UI mode properties
 *                   Returns default values if file doesn't exist or is invalid
 */
function load() {
  return readJsonFile(file, { 
    width: 1000, 
    height: 700,
    x: undefined,  // Let Electron choose default position
    y: undefined,  // Let Electron choose default position
    isRTL: false,  // Default to LTR mode
    isDark: false  // Default to light mode
  });
}

/**
 * Saves the current window state to disk
 * @param {Object} state - Window state object containing position, size, and UI settings
 */
function save(state) {
  // Load existing state to preserve UI settings if not provided
  const existingState = load();
  const mergedState = {
    ...existingState,
    ...state
  };
  writeJsonFile(file, mergedState, false); // No pretty print needed for simple state
}

/**
 * Saves only the UI mode settings (RTL and dark mode)
 * @param {Object} uiSettings - Object containing isRTL and/or isDark properties
 */
function saveUISettings(uiSettings) {
  const currentState = load();
  const updatedState = {
    ...currentState,
    ...uiSettings
  };
  writeJsonFile(file, updatedState, false);
}

// Export all functions for use in main process
module.exports = { load, save, saveUISettings };