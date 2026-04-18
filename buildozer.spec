[app]

# Application title
title = Mathematica Console

# Package name
package.name = mathemaconsole

# Package domain (unique identifier)
package.domain = org.mathematica.console

# Source code location
source.dir = src/mathematica_console

# Requirements
requirements = python3,kivy==2.2.1,kivymd==1.1.1,sympy==1.12,mpmath==1.3.0,matplotlib==3.7.2,numpy==1.24.3,Pillow==10.0.1

# Permissions
android.permissions = INTERNET,WRITE_EXTERNAL_STORAGE,READ_EXTERNAL_STORAGE

# Android API level
android.api = 30
android.minapi = 21
android.ndk = 23b
android.sdk = 30

# App orientation
android.orientation = portrait

# Presplash and icon
# presplash.filename = %(source.dir)s/presplash.png
# icon.filename = %(source.dir)s/icon.png

# Include extensions
source.include_exts = py,png,jpg,kv,atlas,ttf,gif

# Fullscreen
fullscreen = 0

# Window size
window.size = (400, 700)

# Log level
log_level = 2

# Debug mode
debug = 0

[buildozer]

log_level = 2

warn_on_root = 1