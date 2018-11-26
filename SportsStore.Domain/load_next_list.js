var page = require('webpage').create();
var fs = require('fs');
console.log('The default user agent is ' + page.settings.userAgent);
//page.settings.userAgent = 'SpecialAgent';
page.settings.javascriptEnabled = true;
//page.settings.resourceTimeout = 3000;
page.settings.loadImages = false;

page.onConsoleMessage = function(msg) { console.log(msg); }
page.open('https://www.next.co.uk/shop/gender-newborngirls-gender-oldergirls-gender-youngergirls-category-dresses', function () 
{
    var totalCount = page.evaluate(function()
    {
        var ss = document.getElementsByClassName('ResultCount')[0].textContent.trim()
        return parseInt(ss);
    });
    console.log('---A total of ' + totalCount + ' items'); 
    var counter = 0;

    var timer = window.setInterval(function() 
    {
        counter = counter + 1;
        var currentCount = page.evaluate(function() 
        {
            var count = document.getElementsByClassName('Item Fashion').length;
            console.log(count + ' items loaded');
            document.body.scrollTop = document.body.scrollHeight;
            return count;
        });
        
        console.log(currentCount + '<=' + totalCount);
        if (currentCount >= totalCount)
        {
            clearInterval(timer);
            fs.write('c:\\temp\\1.html', page.content, 'w');
            phantom.exit();    
        }
    }, 1000); 

});