[app]

# (str) Title of your application
title = Jupyter IDE

# (str) Package name
package.name = jupyteride

# (str) Package domain (needed for android/ios packaging)
package.domain = org.ide

# (str) Source code where the main.py live
source.dir = .

# (list) Source files to include (let empty to include all the files)
source.include_exts = py,png,jpg,kv,atlas,ttf

# (list) Application requirements
requirements = python3,kivy==2.1.0,pygments

# (str) Supported orientation (one of landscape, sensorLandscape, portrait or all)
orientation = portrait

# (bool) Indicate if the application should be fullscreen or not
fullscreen = 0

# (list) Permissions
android.permissions = WRITE_EXTERNAL_STORAGE,READ_EXTERNAL_STORAGE

# (int) Android API level (the minimum API version)
android.api = 30

# (int) Minimum API required (21 = Android 5.0)
android.minapi = 21

# (int) Android NDK version to use
android.ndk = 25b                    # Changed from 23b to 25b

# (bool) Accept SDK license
android.accept_sdk_license = True

# (str) Android NDK directory (if empty, it will be automatically downloaded.)
android.ndk_path = 

# (str) Android SDK directory (if empty, it will be automatically downloaded.)
android.sdk_path = 

# (str) Android entry point
android.entrypoint = org.kivy.android.PythonActivity

# (str) Android app theme
android.apptheme = @android:style/Theme.NoTitleBar

# (list) Android logcat filters
android.logcat_filters = *:S python:D

# (bool) Copy prebuilt python dependency
android.copy_libs = 1

# (str) Android arch to build for
android.arch = arm64-v8a

# (bool) Enable AndroidX support
android.enable_androidx = True

# (str) Application versioning
version = 0.1

[buildozer]

# (int) Log level
log_level = 2

# (bool) WARN: Stop build if Python file missing module
warn_on_root = 1