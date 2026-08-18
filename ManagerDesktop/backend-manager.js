// backend-manager.js - Backend server management utilities
const { spawn } = require('child_process');
const net = require('net');
const path = require('path');
const { app } = require('electron');
const { loadConfig } = require('./config-manager');

/**
 * Helper function to resolve paths differently for development vs packaged app
 * In development: uses app directory, In production: uses resources directory
 * @param {...string} p - Path segments to join
 * @returns {string} Resolved path
 */
function resolvePath(...p) {
  return app.isPackaged 
    ? path.join(process.resourcesPath, ...p) 
    : path.join(app.getAppPath(), ...p);
}

/**
 * Waits for a TCP port to become available (backend server to start)
 * @param {number} port - Port number to check
 * @returns {Promise} Resolves when port is available
 */
function waitForPort(port) {
  return new Promise((resolve) => {
    (function tick() {
      const socket = net.connect(port, '127.0.0.1');
      socket.once('connect', () => { 
        socket.end(); 
        resolve(); 
      });
      socket.once('error', () => setTimeout(tick, 200));
    })();
  });
}

/**
 * Finds a free port in the dynamic range (49152-65535)
 * @returns {Promise<number>} Resolves with a free port number
 */
function findFreePort() {
  return new Promise((resolve) => {
    const checkPort = () => {
      const port = Math.floor(Math.random() * (65535 - 49152 + 1)) + 49152;
      const server = net.createServer();
      
      server.once('error', () => {
        checkPort();
      });
      
      server.once('listening', () => {
        server.close(() => {
          resolve(port);
        });
      });
      
      server.listen(port, '127.0.0.1');
    };
    
    checkPort();
  });
}

/**
 * Tries to use the preferred port, falls back to random port if unavailable
 * @param {number} preferredPort - The preferred port number to try first
 * @returns {Promise<number>} Resolves with the selected port number
 */
function findPortWithPreference(preferredPort) {
  return new Promise((resolve) => {
    const server = net.createServer();
    
    server.once('error', async () => {
      const fallbackPort = await findFreePort();
      resolve(fallbackPort);
    });
    
    server.once('listening', () => {
      server.close(() => {
        resolve(preferredPort);
      });
    });
    
    server.listen(preferredPort, '127.0.0.1');
  });
}

/**
 * Starts the backend server process
 * @param {number} port - Port number for the server
 * @returns {ChildProcess} The spawned backend process
 */
function startBackend(port) {
  // Platform-specific executable name
  const exe = process.platform === 'win32' ? 'ManagerServer.exe' : 'ManagerServer';
  const exePath = resolvePath('ManagerServer', exe);
  console.log(exePath);
  
  // Build command arguments
  const args = [`--urls=http://127.0.0.1:${port}`, '--edition=desktop'];
  
  // Add custom data path if configured
  const config = loadConfig();
  if (config.dataPath) {
    args.push(`--path=${config.dataPath}`);
    console.log(`Using custom data path: ${config.dataPath}`);
  }
  
  // Spawn backend server process
  const backend = spawn(exePath, args, { stdio: 'ignore' });

  return backend;
}

module.exports = {
  resolvePath,
  waitForPort,
  findFreePort,
  findPortWithPreference,
  startBackend
};