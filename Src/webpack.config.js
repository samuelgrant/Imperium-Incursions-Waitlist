const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CleanWebpackPlugin = require('clean-webpack-plugin');
var WebpackNotifierPlugin = require("webpack-notifier");
var BrowserSyncPlugin = require("browser-sync-webpack-plugin");

const bundleFileName = 'bundle';
const dirName = 'wwwroot/dist';

module.exports = (env, argv) => {
    return {
        mode: argv.mode === "production" ? "production" : "development",
        entry: ['./Resources/index.js', './Resources/sass/index.scss'],
        output: {
            filename: bundleFileName + '.js',
            path: path.resolve(__dirname, dirName)
        },
        module: {
            rules: [
                {
                    test: /\.s[c|a]ss$/,
                    use:
                        [
                            'style-loader',
                            MiniCssExtractPlugin.loader,
                            'css-loader',
                            {
                                loader: 'postcss-loader',
                                options: {
                                    config: {
                                        ctx: {
                                            env: argv.mode
                                        }
                                    }
                                }
                            },
                            'sass-loader'
                        ]
                },
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: {
                        loader: "babel-loader"
                    }
                },
            ]
        },
        devtool: "inline-source-map",
        plugins: [
            new CleanWebpackPlugin(dirName, {}),
            new MiniCssExtractPlugin({
                filename: bundleFileName + '.css'
            }),
            new WebpackNotifierPlugin(),
            new BrowserSyncPlugin()
        ],
        stats: {
            colors: true,
            hash: false,
            version: false,
            timings: false,
            assets: false,
            chunks: false,
            modules: false,
            reasons: false,
            children: false,
            source: false,
            errors: false,
            errorDetails: false,
            warnings: false,
            publicPath: false
        }
    };
};