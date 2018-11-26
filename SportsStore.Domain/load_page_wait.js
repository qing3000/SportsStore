var page = require('webpage').create();
var fs = require('fs');
page.settings.javascriptEnabled = true;
page.settings.loadImages = false;

page.open('URL', function (status)
{
    if (status !== 'success')
    {
        console.log('Unable to load the address!');
        phantom.exit();
    }
    else
    {
        window.setTimeout(function ()
        {
            fs.write('OUTPUT_FILE', page.content, 'w');
            phantom.exit();
        }, 2000); // Change timeout as required to allow sufficient time 
    }
});