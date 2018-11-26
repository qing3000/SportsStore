var page = require('webpage').create();
var fs = require('fs');
page.settings.javascriptEnabled = true;
page.settings.loadImages = false;

function onPageReady()
{
    var htmlContent = page.evaluate(function ()
    {
        return document.documentElement.outerHTML;
    });
    fs.write('OUTPUT_FILE', htmlContent, 'w');
    phantom.exit();
}

page.open('URL', function (status)
{
    function checkReadyState()
    {
        setTimeout(function ()
        {
            var readyState = page.evaluate(function ()
            {
                return document.getElementsByClassName('ThumbNailNavClip')[0].getElementsByTagName('li').length;
            });

            if (readyState > 0)
            {
                onPageReady();
            }
            else
            {
                checkReadyState();
            }
        });
    }

    checkReadyState();
});