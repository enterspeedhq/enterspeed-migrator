/// <binding ProjectOpened='default' />
//
const {watch, src, dest} = require('gulp');
var config = require('./paths.json');

/*
 * app_plugin and build script.
 */

const appPluginPath = '/App_Plugins/' + config.pluginFolder;

const appPlugin = {
    source: config.library + appPluginPath + '/**/*',
    src: config.library + appPluginPath + '/',
    dest: config.site + appPluginPath
}

function copy(path, baseFolder, target) {

    console.log('copy: \x1b[36m%s\x1b[0m %s', path, target);

    return src(path, {base: baseFolder})
        .pipe(dest(target));
}


function watchAppPlugins() {

    console.log();
    console.log('Watching : ' + appPlugin.source);
    console.log('Target   : ' + appPlugin.dest);

    watch(appPlugin.source, {ignoreInitial: false})
        .on('change', function (path, stats) {
            console.log("Copying files ");
            copy(path, appPlugin.src, appPlugin.dest)
        })
        .on('add', function (path, stats) {
            console.log("Copying files ");
            copy(path, appPlugin.src, appPlugin.dest)
        });
}

exports.default = function () {
    watchAppPlugins();
};