// download-server.js - Downloads the ManagerServer backend from the latest GitHub release
const fs = require('fs');
const path = require('path');
const { spawnSync } = require('child_process');

const RUNTIMES = ['win-x64', 'win-arm64', 'osx-x64', 'osx-arm64', 'linux-x64', 'linux-arm64'];
const PLATFORMS = { win32: 'win', darwin: 'osx', linux: 'linux' };

// Runtime can be passed explicitly (e.g. `npm run download-server -- osx-x64`
// for cross-builds in CI), otherwise it's detected from the current machine.
const runtime = process.argv[2] || `${PLATFORMS[process.platform]}-${process.arch}`;

const ext = runtime.startsWith('linux-') ? 'tar.gz' : 'zip';
const asset = `ManagerServer-${runtime}.${ext}`;
const url = `https://github.com/managerhq/Manager/releases/latest/download/${asset}`;
const targetDir = path.join(__dirname, 'ManagerServer');
const archivePath = path.join(__dirname, asset);

async function main() {
  if (!RUNTIMES.includes(runtime)) {
    throw new Error(`Unsupported runtime: ${runtime} (expected one of ${RUNTIMES.join(', ')})`);
  }

  console.log(`Downloading ${url}`);
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error(`Download failed: ${response.status} ${response.statusText}`);
  }
  fs.writeFileSync(archivePath, Buffer.from(await response.arrayBuffer()));

  fs.rmSync(targetDir, { recursive: true, force: true });
  fs.mkdirSync(targetDir);

  const result = spawnSync('tar', ['-xf', archivePath, '-C', targetDir], { stdio: 'inherit' });
  if (result.status !== 0) {
    throw new Error('Extraction failed');
  }

  if (!runtime.startsWith('win-')) {
    fs.chmodSync(path.join(targetDir, 'ManagerServer'), 0o755);
  }

  console.log(`ManagerServer extracted to ${targetDir}`);
}

main()
  .catch((err) => {
    console.error(err.message);
    process.exitCode = 1;
  })
  .finally(() => fs.rmSync(archivePath, { force: true }));
