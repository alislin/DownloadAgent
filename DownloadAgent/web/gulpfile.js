const { dest, src, watch, series, clean } = require("gulp");

function publishTask() {
    return src(["public/**/*.js","public/**/*.map","public/**/*.css","public/*"])
        .pipe(dest("../wwwroot/"));
}

function watchTask() {

    return watch("public/**/*", { delay: 500 }, publishTask);
}
exports.publish = publishTask;
exports.default = watchTask;