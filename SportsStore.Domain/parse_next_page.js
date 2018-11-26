var page = require('webpage').create();
var fs = require('fs');
console.log('The default user agent is ' + page.settings.userAgent);
//page.settings.userAgent = 'SpecialAgent';
page.settings.javascriptEnabled = true;
page.settings.loadImages = false;
//page.settings.resourceTimeout = 3000;

//page.onConsoleMessage = function(msg) { console.log(msg); }
page.open('http://www.next.co.uk/g92236s7#322425', 
    function(status) 
    {
        if (status !== 'success')
        {
            console.log('Unable to access network');
        }
        else 
        {
            fs.write('c:\\temp\\2.html', page.content, 'w');

        }
        phantom.exit();
    });