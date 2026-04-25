import { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'com.yourcompany.pythonnotebook',
  appName: 'Python Notebook',
  webDir: 'frontend/dist',          // Your built web files location
  bundledWebRuntime: false,
  
  server: {
    androidScheme: 'https',
    cleartext: true
  },
  
  android: {
    buildOptions: {
      keystorePath: 'release.jks',
      keystorePassword: process.env.KEYSTORE_PASSWORD,
      keystoreAlias: process.env.KEY_ALIAS,
      keystoreAliasPassword: process.env.KEY_PASSWORD,
    }
  }
};

export default config;
