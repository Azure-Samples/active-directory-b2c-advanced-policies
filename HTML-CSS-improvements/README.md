---
title: Removing Flash on Loading B2C Pages | Microsoft Docs
description: Learn how to eliminate the visible flash when loading content on a b2c page by minifying and inlining the CSS in a style element in the header
services: active-directory-b2c
documentationcenter: ''
author: rojasja
manager: alexsi
editor: ''

ms.assetid: 
ms.service: active-directory-b2c
ms.workload: identity
ms.tgt_pltfrm: na
ms.devlang: na
ms.topic: troubleshooting
ms.date: 01/08/2018
ms.author: joroja

---
# How to eliminate the flash in loading user journey content in a browser with Azure Active Directory B2C
Azure Active Directory B2C loads user created content in HTML, CSS and javascript along with its own content to create fully customizable user interaction pages to achieve identity tasks like signup, signin, password reset, and profile edit among many possible variations.  When loading content in the browser, it is ocassionally observed that the HTML code precedes the CSS style and content resulting in a visible flash of unstyled content before rendering the page as intended.  This detracts from the user experience and may be highlighted with users using slow connections and, or higher than average content quantity.


## The solution is to minify and inline the CSS
The issue is solved by minifying and inlining the CSS in a style element in the header.  This is also known as compressing and internalizing what may otherwise be externalized content (such as CSS) into the HTML file.  This will avoid the delay of loading the externalized cotnent and prevent the page "flash" or "flicker."  We recommend that that this is performed on the final version of content before production.  Minimized CSS is much harder to edit.


## Manual approach
If done manually, the task involves concatenating and minifying all of the CSS files and then "injecting" it into each of the HTML files which are used for the templates.  Online tools are readily available to perform these tasks if not already included in your HTML IDE.  Since the HTML files will have all embedded content, it will load and be displayed correctly at once.

## Script using Gulp.js
Advanced users may wish to automate the task using a task/build runner.  The sample gulp.js script below will concatenate and minify all of the CSS files and then "inject" them into each of the HTML files which are sued for templates.

```gulp
var gulp = require('gulp');
var concat = require('gulp-concat');
var cssmin = require('gulp-cssmin');
var inject = require('gulp-inject');
var removeCode = require('gulp-remove-code');

gulp.task('default', function() {
    // Concatenate and minify the style files.
    var cssStream = gulp.src(['./src/css/bootstrap.css', './src/css/global.css'])
        .pipe(concat('all.css'))
        .pipe(cssmin());

    // Copy the font files.
    gulp.src(['./src/fonts/*.*'])
        .pipe(gulp.dest('./dist/fonts'));

    // Copy the image files.
    gulp.src(['./src/images/*.*'])
        .pipe(gulp.dest('./dist/images'));

    // Copy the script files.
    gulp.src(['./src/js/*.*'])
        .pipe(gulp.dest('./dist/js'));

    // Inject the concatenated, minified, script files into each of the HTML files and remove any external files.
    return gulp.src(['./src/*.html'])
        .pipe(inject(cssStream, {
            transform: function (filePath, file) {
                return '<style>' + file.contents.toString('utf8') + '</style>';
            }
        }))
        .pipe(removeCode({
            production: true
        }))
        .pipe(gulp.dest('./dist'));
});

```



## Next steps
N/A
