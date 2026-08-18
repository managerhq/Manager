// tab-utils.js - Utility functions for tab-related operations

/**
 * Parses a tab title to extract business name and screen name
 * @param {string} title - The full tab title
 * @returns {Object} Object with businessName and screenName properties
 */
function parseTabTitle(title) {
  const fullTitle = title || 'New Tab';
  let businessName = '';
  let screenName = fullTitle;
  
  // Split title by pipe character
  if (fullTitle.includes('|')) {
    const parts = fullTitle.split('|').map(p => p.trim());
    if (parts.length >= 2) {
      businessName = parts[0];
      screenName = parts.slice(1).join(' | '); // Join remaining parts in case there are multiple pipes
    }
  }
  
  return {
    businessName,
    screenName,
    fullTitle
  };
}

/**
 * Checks if a URL is external (not localhost or file://)
 * @param {string} url - The URL to check
 * @param {number} port - The localhost port number
 * @returns {boolean} True if the URL is external
 */
function isExternalUrl(url, port) {
  return !url.startsWith(`http://127.0.0.1:${port}`) && !url.startsWith('file://');
}

module.exports = {
  parseTabTitle,
  isExternalUrl
};