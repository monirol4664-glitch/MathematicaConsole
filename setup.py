from setuptools import setup, find_packages

with open("README.md", "r", encoding="utf-8") as fh:
    long_description = fh.read()

setup(
    name="mathematica-console",
    version="1.0.0",
    author="Your Name",
    author_email="your@email.com",
    description="Offline Mathematica-style symbolic math console for Android",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://github.com/yourusername/mathematica-console",
    package_dir={"": "src"},
    packages=find_packages(where="src"),
    classifiers=[
        "Development Status :: 4 - Beta",
        "Intended Audience :: Education",
        "Intended Audience :: Science/Research",
        "License :: OSI Approved :: MIT License",
        "Operating System :: Android",
        "Programming Language :: Python :: 3",
        "Programming Language :: Python :: 3.9",
        "Programming Language :: Python :: 3.10",
        "Programming Language :: Python :: 3.11",
        "Topic :: Scientific/Engineering :: Mathematics",
        "Topic :: Education",
    ],
    python_requires=">=3.9",
    install_requires=[
        "kivy>=2.2.1",
        "kivymd>=1.1.1",
        "sympy>=1.12",
        "mpmath>=1.3.0",
        "matplotlib>=3.7.0",
        "numpy>=1.24.0",
        "pillow>=10.0.0",
    ],
    include_package_data=True,
    entry_points={
        "console_scripts": [
            "mathematica-console=mathematica_console.main:main",
        ],
    },
)