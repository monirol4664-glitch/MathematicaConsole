# Mathematica Console - Offline Android App

A complete offline symbolic mathematics console for Android, inspired by Wolfram Mathematica.

## Features

- 🔢 **Symbolic Mathematics** - Integration, differentiation, solving equations, limits, series
- 📊 **2D & 3D Plotting** - Visualize functions offline
- 💾 **Persistent History** - Command history saved between sessions
- 🎯 **Smart Autocomplete** - Mathematica-style function suggestions
- 📱 **Touch-Optimized** - Designed for mobile devices
- 🔒 **100% Offline** - No internet connection required

## Installation

### From GitHub Actions
1. Go to Actions tab in your repository
2. Select "Build Mathematica Console APK"
3. Click "Run workflow"
4. Download APK from artifacts

### Manual Build
```bash
# Install buildozer
pip install buildozer

# Build APK
buildozer android debug

# Output APK in bin/ directory