// config-manager.js - Configuration management utilities
const path = require('path');
const { app } = require('electron');
const { readJsonFile, writeJsonFile } = require('./json-file-utils');

/**
 * Get the path to the configuration file
 * @returns {string} Path to config.json
 */
function getConfigPath() {
  return path.join(app.getPath('userData'), 'config.json');
}

/**
 * Loads the application configuration
 * @returns {Object} Configuration object with dataPath property
 */
function loadConfig() {
  return readJsonFile(getConfigPath(), {});
}

/**
 * Saves the application configuration
 * @param {Object} config - Configuration object to save
 */
function saveConfig(config) {
  writeJsonFile(getConfigPath(), config, true); // Pretty print for readability
}

module.exports = {
  getConfigPath,
  loadConfig,
  saveConfig
};