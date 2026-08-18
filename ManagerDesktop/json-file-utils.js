// json-file-utils.js - Utility functions for JSON file operations
const fs = require('fs');

/**
 * Reads and parses a JSON file
 * @param {string} filePath - Path to the JSON file
 * @param {*} defaultValue - Default value to return if file doesn't exist or is invalid
 * @returns {*} Parsed JSON object or defaultValue
 */
function readJsonFile(filePath, defaultValue = {}) {
  try {
    if (fs.existsSync(filePath)) {
      const data = fs.readFileSync(filePath, 'utf8');
      return JSON.parse(data);
    }
  } catch (error) {
    console.error(`Error reading JSON file ${filePath}:`, error);
  }
  return defaultValue;
}

/**
 * Writes an object to a JSON file
 * @param {string} filePath - Path to the JSON file
 * @param {*} data - Data to write
 * @param {boolean} pretty - Whether to format the JSON (default: true)
 * @returns {boolean} True if successful, false otherwise
 */
function writeJsonFile(filePath, data, pretty = true) {
  try {
    const jsonString = pretty 
      ? JSON.stringify(data, null, 2) 
      : JSON.stringify(data);
    fs.writeFileSync(filePath, jsonString);
    return true;
  } catch (error) {
    console.error(`Error writing JSON file ${filePath}:`, error);
    return false;
  }
}

module.exports = {
  readJsonFile,
  writeJsonFile
};