var page = require('webpage').create();
var fs = require('fs');
console.log('The default user agent is ' + page.settings.userAgent);
page.settings.userAgent = 'SpecialAgent';
page.settings.javascriptEnabled = true;
//page.settings.resourceTimeout = 3000;
page.settings.loadImages = true;

//page.onConsoleMessage = function(msg) { console.log(msg); }
page.open('URL', function(status) 
{
    if (status !== 'success')
    {
        console.log('Unable to access network');
    }
    else 
    {
	fs.write('OUTPUT_FILE', page.content, 'w');
    }
    phantom.exit();
});