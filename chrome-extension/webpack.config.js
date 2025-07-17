const path = require('path');
const CopyPlugin = require('copy-webpack-plugin');
const webpack = require('webpack');

module.exports = {
  mode: 'production',
  entry: {
    background: './src/background.js',
    popup: './src/popup.js',
    pin: './src/pin.js',
    content: './src/content.js'
  },
  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: '[name].js',
    clean: true
  },
  target: ['web', 'es2020'],
  plugins: [
    new CopyPlugin({
      patterns: [
        { from: 'src/*.png', to: '[name][ext]' },
        { from: 'src/*.ico', to: '[name][ext]' },
        { from: "manifest.json", to: "./" },
        { from: "src/popup.html", to: "./" },
        { from: "src/pin.html", to: "./" }
      ],
    }),
    new webpack.ProvidePlugin({
      Buffer: ['buffer', 'Buffer']
    }),
    new webpack.DefinePlugin({
      'process.env.NODE_ENV': JSON.stringify('production'),
      'process.env': JSON.stringify({}),
      'process': 'undefined', // <-- Critical Fix
    }),
  ],
  resolve: {
    fallback: {
      crypto: false,
      stream: false,
      buffer: false,
      util: false,
      assert: false,
      process: false,
    }
  }
};
