// print-preview.js - Print preview window with live settings
const { BrowserWindow, WebContentsView, ipcMain, dialog, shell } = require('electron');
const fs = require('fs');
const path = require('path');
const { app } = require('electron');

class PrintPreview {
  constructor(sourceWindow) {
    this.sourceWindow = sourceWindow;
    this.iframeContent = sourceWindow.iframeContent || null; // Store iframe content if provided
    this.previewWindow = null;
    this.pdfView = null;
    this.currentSettings = {
      landscape: false,
      pageSize: 'A4',
      margins: {
        marginType: 'default',
        top: 0.6,
        bottom: 0.6,
        left: 0.6,
        right: 0.6
      },
      printBackground: true,
      scale: 1
    };
    this.tempFiles = [];
  }

  async show() {
    // Determine the parent window for modal behavior
    // sourceWindow could be either a BrowserWindow or our custom wrapper object
    const parentWindow = this.sourceWindow.parent || this.sourceWindow;
    
    // Create the preview window
    // On macOS, modal windows with parent don't show title bars (they become sheets)
    // So we conditionally set modal behavior
    const isMac = process.platform === 'darwin';
    
    this.previewWindow = new BrowserWindow({
      width: 1200,
      height: 800,
      parent: parentWindow,
      modal: !isMac,  // Only modal on non-Mac platforms
      frame: true,  // Ensure frame is shown
      titleBarStyle: 'default',  // Force default title bar style
      autoHideMenuBar: true,
      webPreferences: {
        nodeIntegration: false,  // Disabled for security
        contextIsolation: true,   // Enabled for security
        preload: path.join(__dirname, 'print-preview-preload.js')  // Use secure preload script
      },
      title: 'Print Preview',
      closable: true,
      minimizable: false,
      maximizable: true,
      // Ensure window appears centered over parent
      center: true
    });
    
    // On macOS, manually implement modal-like behavior
    if (isMac && parentWindow) {
      // Make parent window unfocusable while preview is open
      parentWindow.setFocusable(false);
      
      // Restore focus capability when preview closes
      this.previewWindow.on('closed', () => {
        if (!parentWindow.isDestroyed()) {
          parentWindow.setFocusable(true);
          parentWindow.focus();
        }
      });
    }

    // Create WebContentsView for PDF display
    this.pdfView = new WebContentsView({
      webPreferences: {
        contextIsolation: true,
        nodeIntegration: false
      }
    });
    
    // Load the print preview HTML (for controls)
    const previewHtml = this.generatePreviewHtml();
    const tempHtmlPath = path.join(app.getPath('temp'), `print-preview-${Date.now()}.html`);
    fs.writeFileSync(tempHtmlPath, previewHtml);
    this.tempFiles.push(tempHtmlPath);
    
    await this.previewWindow.loadFile(tempHtmlPath);
    
    // Add the PDF view to the window
    this.previewWindow.contentView.addChildView(this.pdfView);
    
    // Set initial bounds for the PDF view (will be adjusted by the renderer)
    const sidebarWidth = 280;
    this.updatePdfViewBounds(sidebarWidth);
    
    // Handle window resize
    this.previewWindow.on('resize', () => {
      this.updatePdfViewBounds(sidebarWidth);
    });
    
    // Set up IPC handlers for this preview session
    this.setupIpcHandlers();
    
    // Generate initial PDF
    await this.updatePreview();
    
    // Clean up when window closes
    this.previewWindow.on('closed', () => {
      this.cleanup();
    });
  }

  updatePdfViewBounds(sidebarWidth) {
    if (this.pdfView && this.previewWindow && !this.previewWindow.isDestroyed()) {
      const bounds = this.previewWindow.getContentBounds();
      this.pdfView.setBounds({ 
        x: 0, 
        y: 0, 
        width: bounds.width - sidebarWidth, 
        height: bounds.height 
      });
    }
  }

  generatePreviewHtml() {
    return `<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <title>Print Preview</title>
  <style>
    * {
      margin: 0;
      padding: 0;
      box-sizing: border-box;
    }
    
    html, body {
      height: 100%;
      overflow: hidden;
    }
    
    body {
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      display: flex;
      flex-direction: row;
      background: #f0f0f0;
    }
    
    #preview-container {
      flex: 1;
      display: flex;
      flex-direction: column;
      min-width: 0;
      height: 100vh;
    }
    
    #sidebar {
      width: 280px;
      background: white;
      border-left: 1px solid #ddd;
      padding: 20px;
      overflow-y: auto;
      flex: 0 0 280px;
      display: flex;
      flex-direction: column;
      gap: 20px;
      order: 2;
    }
    
    /* PDF viewer has its own print/save controls */
    
    #pdf-viewer {
      flex: 1;
      display: flex;
      min-height: 0;
      overflow: hidden;
      position: relative;
    }
    
    .setting-card {
      background: #f8f9fa;
      border-radius: 8px;
      padding: 12px;
      display: flex;
      flex-direction: column;
      gap: 12px;
      min-width: 0;
    }
    
    .setting-row {
      display: flex;
      align-items: center;
      gap: 12px;
      min-width: 0;
    }
    
    .icon {
      width: 20px;
      height: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #666;
      flex-shrink: 0;
    }
    
    select {
      flex: 1;
      padding: 6px 10px;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 13px;
      background: white;
      cursor: pointer;
    }
    
    .toggle-button {
      width: 40px;
      height: 40px;
      border: 1px solid #ddd;
      border-radius: 4px;
      background: white;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s;
    }
    
    .toggle-button:hover {
      background: #f0f0f0;
    }
    
    .toggle-button.active {
      background: #007bff;
      color: white;
      border-color: #007bff;
    }
    
    .scale-slider {
      display: flex;
      align-items: center;
      gap: 10px;
      flex: 1;
      min-width: 0;
    }
    
    input[type="range"] {
      flex: 1;
      height: 4px;
      border-radius: 2px;
      outline: none;
      -webkit-appearance: none;
      background: #ddd;
      min-width: 0;
    }
    
    input[type="range"]::-webkit-slider-thumb {
      -webkit-appearance: none;
      width: 16px;
      height: 16px;
      border-radius: 50%;
      background: #007bff;
      cursor: pointer;
    }
    
    input[type="range"]::-moz-range-thumb {
      width: 16px;
      height: 16px;
      border-radius: 50%;
      background: #007bff;
      cursor: pointer;
      border: none;
    }
    
    .scale-value {
      font-size: 13px;
      color: #666;
      min-width: 40px;
      text-align: right;
    }
    
    .divider {
      height: 1px;
      background: #e0e0e0;
      margin: 4px 0;
    }
    
    
    #loading {
      position: absolute;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      background: white;
      padding: 20px;
      border-radius: 8px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
      font-size: 14px;
      color: #666;
      display: none;
      z-index: 10;
      pointer-events: none;
    }
    
    #loading.active {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    
    .spinner {
      width: 20px;
      height: 20px;
      border: 3px solid #f0f0f0;
      border-top: 3px solid #007bff;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }
    
    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
    
  </style>
</head>
<body>
  <div id="preview-container">
    <div id="pdf-viewer">
      <div id="loading">
        <div class="spinner"></div>
        <span>Updating preview...</span>
      </div>
      <!-- PDF is now displayed via WebContentsView, not webview tag -->
    </div>
  </div>
  
  <div id="sidebar">
    <!-- Page Size -->
    <div class="setting-card">
      <div class="setting-row">
        <div class="icon">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="5" y="3" width="14" height="18" rx="1"/>
            <line x1="9" y1="7" x2="15" y2="7"/>
            <line x1="9" y1="11" x2="15" y2="11"/>
          </svg>
        </div>
        <select id="pageSize" onchange="updateSettings()">
          <option value="A4">A4</option>
          <option value="Letter">Letter</option>
          <option value="Legal">Legal</option>
          <option value="A3">A3</option>
          <option value="A5">A5</option>
          <option value="Tabloid">Tabloid</option>
        </select>
      </div>
    </div>
    
    <!-- Orientation -->
    <div class="setting-card">
      <div class="setting-row">
        <div class="icon">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="7" y="3" width="10" height="14" rx="1" fill="none"/>
            <rect x="3" y="7" width="14" height="10" rx="1" fill="none" opacity="0.5"/>
          </svg>
        </div>
        <button class="toggle-button active" id="portrait" onclick="setOrientation(false)" title="Portrait">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="6" y="3" width="12" height="18" rx="1"/>
          </svg>
        </button>
        <button class="toggle-button" id="landscape-btn" onclick="setOrientation(true)" title="Landscape">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="6" width="18" height="12" rx="1"/>
          </svg>
        </button>
      </div>
    </div>
    
    <!-- Scale -->
    <div class="setting-card">
      <div class="setting-row">
        <div class="icon">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="11" cy="11" r="8"/>
            <line x1="21" y1="21" x2="16.65" y2="16.65"/>
            <line x1="11" y1="8" x2="11" y2="14"/>
            <line x1="8" y1="11" x2="14" y2="11"/>
          </svg>
        </div>
        <div class="scale-slider">
          <input type="range" id="scale" min="50" max="200" value="100" onchange="updateSettings()">
          <span class="scale-value" id="scaleValue">100%</span>
        </div>
      </div>
    </div>
  </div>
  
  <script>
    // Wait for DOM and electronAPI to be ready
    let electronAPIRef;  // Using electronAPIRef to avoid conflict with global electronAPI
    let updateTimer = null;
    let isLandscape = false;
    let waitCount = 0;
    
    // Check if electronAPI is available
    function waitForElectronAPI() {
      waitCount++;
      
      if (window.electronAPI) {
        electronAPIRef = window.electronAPI;
        // Initialize after API is ready
        initialize();
      } else {
        if (waitCount > 100) {
          console.error('Failed to find electronAPI after 100 attempts');
          return;
        }
        setTimeout(waitForElectronAPI, 10);
      }
    }
    
    // Initialize once API is ready
    function initialize() {
      // Initial load indicator
      showLoading();
      // Setup PDF listener
      setupPdfListener();
    }
    
    // Handle ESC key to close window
    document.addEventListener('keydown', function(event) {
      if (event.key === 'Escape') {
        window.close();
      }
    });
    
    function setOrientation(landscape) {
      isLandscape = landscape;
      document.getElementById('portrait').classList.toggle('active', !landscape);
      document.getElementById('landscape-btn').classList.toggle('active', landscape);
      updateSettings();
    }
    
    function updateSettings() {
      // Update scale value display
      const scaleInput = document.getElementById('scale');
      document.getElementById('scaleValue').textContent = scaleInput.value + '%';
      
      // Debounce updates to avoid too many regenerations
      clearTimeout(updateTimer);
      updateTimer = setTimeout(() => {
        const settings = {
          landscape: isLandscape,
          pageSize: document.getElementById('pageSize').value,
          margins: { marginType: 'default' },
          printBackground: true,
          scale: parseInt(document.getElementById('scale').value) / 100
        };
        
        if (electronAPIRef && electronAPIRef.sendPrintPreviewUpdate) {
          electronAPIRef.sendPrintPreviewUpdate(settings);
          showLoading();
        } else {
          console.error('electronAPI not available');
        }
      }, 300);
    }
    
    function showLoading() {
      document.getElementById('loading').classList.add('active');
    }
    
    function hideLoading() {
      document.getElementById('loading').classList.remove('active');
    }
    
    
    // Setup PDF listener once API is available
    function setupPdfListener() {
      if (electronAPIRef && electronAPIRef.onPrintPreviewPdfReady) {
        electronAPIRef.onPrintPreviewPdfReady(() => {
          // PDF is loaded in WebContentsView, just hide the loading indicator
          setTimeout(() => {
            hideLoading();
          }, 100);
        });
      } else {
        console.error('Cannot setup PDF listener - API not available');
      }
    }
    
    // Initial load
    window.addEventListener('DOMContentLoaded', () => {
      waitForElectronAPI();
    });
    
    // Also call immediately if DOM is already loaded
    if (document.readyState !== 'loading') {
      waitForElectronAPI();
    }
    
    // Clean up listener on window unload
    window.addEventListener('beforeunload', () => {
      if (electronAPIRef && electronAPIRef.removePrintPreviewPdfReadyListener) {
        electronAPIRef.removePrintPreviewPdfReadyListener();
      }
    });
  </script>
</body>
</html>`;
  }

  setupIpcHandlers() {
    // Handle settings updates
    const updateHandler = async (event, settings) => {
      if (event.sender === this.previewWindow.webContents) {
        this.currentSettings = { ...this.currentSettings, ...settings };
        await this.updatePreview();
      }
    };
    
    // Register handlers
    ipcMain.on('print-preview-update', updateHandler);
    
    // Clean up handlers when window closes
    this.previewWindow.on('closed', () => {
      ipcMain.removeListener('print-preview-update', updateHandler);
    });
  }

  async updatePreview() {
    try {
      let pdfData;
      let filename = 'document';
      
      // Check if we're printing iframe content
      if (this.iframeContent) {
        
        // Create a hidden window to render the iframe content
        // JavaScript is disabled — the iframe content is prerendered HTML
        const hiddenWindow = new BrowserWindow({
          show: false,
          webPreferences: {
            offscreen: true,
            javascript: false
          }
        });
        
        // Write the iframe HTML to a temp file and load it from disk instead of a data: URL.
        // Large reports can produce multi-megabyte HTML, and a data: URL that size fails to
        // load on macOS with net::ERR_INVALID_URL. A <base> tag preserves resolution of
        // relative paths (images, CSS, fonts) against the backend, same as the
        // baseURLForDataURL option did for the data: URL approach.
        const baseUrl = `http://127.0.0.1:${global.backendPort || 55777}`;
        const baseTag = `<base href="${baseUrl}/">`;
        const htmlWithBase = /<head[^>]*>/i.test(this.iframeContent)
          ? this.iframeContent.replace(/<head[^>]*>/i, match => `${match}${baseTag}`)
          : baseTag + this.iframeContent;

        const tempIframeHtmlPath = path.join(app.getPath('temp'), `print-content-${Date.now()}.html`);
        fs.writeFileSync(tempIframeHtmlPath, htmlWithBase);
        this.tempFiles.push(tempIframeHtmlPath);

        await hiddenWindow.loadFile(tempIframeHtmlPath);
        
        // Wait for content to be ready
        await new Promise(resolve => setTimeout(resolve, 100));
        
        // Get the page title for filename from iframe
        try {
          const pageTitle = await hiddenWindow.webContents.executeJavaScript('document.title');
          if (pageTitle) {
            filename = pageTitle.replace(/[<>:"/\\|?*]/g, '-').trim();
            if (filename.length > 200) {
              filename = filename.substring(0, 200);
            }
          }
        } catch (e) {
          // Could not get iframe title
        }
        
        // Generate PDF with current settings
        const printOptions = {
          landscape: this.currentSettings.landscape,
          pageSize: this.currentSettings.pageSize,
          margins: this.currentSettings.margins,
          printBackground: this.currentSettings.printBackground,
          scale: this.currentSettings.scale,
          preferCSSPageSize: true
        };
        
        pdfData = await hiddenWindow.webContents.printToPDF(printOptions);
        
        // Clean up the hidden window
        hiddenWindow.destroy();
        
      } else {
        // Normal printing from main window
        // Get the page title for filename
        const pageTitle = await this.sourceWindow.webContents.executeJavaScript('document.title');
        
        // Sanitize filename - remove invalid characters
        filename = pageTitle || 'document';
        filename = filename.replace(/[<>:"/\\|?*]/g, '-').trim();
        if (filename.length > 200) {
          filename = filename.substring(0, 200);
        }
        
        // Generate PDF with current settings
        const printOptions = {
          landscape: this.currentSettings.landscape,
          pageSize: this.currentSettings.pageSize,
          margins: this.currentSettings.margins,
          printBackground: this.currentSettings.printBackground,
          scale: this.currentSettings.scale,
          preferCSSPageSize: true
        };
        
        pdfData = await this.sourceWindow.webContents.printToPDF(printOptions);
      }
      
      // Save to temp file with meaningful name but unique timestamp to avoid conflicts
      // This way the display name is meaningful but the actual file is unique
      const timestamp = Date.now();
      const pdfPath = path.join(app.getPath('temp'), `${filename}-${timestamp}.pdf`);
      fs.writeFileSync(pdfPath, pdfData);
      this.tempFiles.push(pdfPath);
      
      // Load PDF directly into the WebContentsView
      if (this.pdfView) {
        await this.pdfView.webContents.loadFile(pdfPath);
        // Notify the main window that loading is complete
        this.previewWindow.webContents.send('print-preview-pdf-ready');
      }
    } catch (error) {
      console.error('Failed to generate preview:', error);
      dialog.showErrorBox('Preview Error', `Failed to generate preview: ${error.message}`);
    }
  }

  cleanup() {
    // Clean up temp files
    this.tempFiles.forEach(file => {
      try {
        fs.unlinkSync(file);
      } catch (e) {
        // Ignore cleanup errors
      }
    });
    
    // Clean up WebContentsView
    if (this.pdfView && !this.pdfView.webContents.isDestroyed()) {
      this.pdfView.webContents.close();
    }
    this.pdfView = null;
    this.previewWindow = null;
  }
}

module.exports = PrintPreview;