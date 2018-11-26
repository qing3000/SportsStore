var page = require('webpage').create();
console.log('The default user agent is ' + page.settings.userAgent);
page.settings.userAgent = 'SpecialAgent';
page.settings.javascriptEnabled = true;
page.settings.loadImages = false;
//page.settings.resourceTimeout = 3000;

//page.onConsoleMessage = function(msg) { console.log(msg); }
page.open('URL', function(status) 
{
    if (status !== 'success')
    {
        console.log('Unable to access network');
    }
    else 
    {
        var sizePrices = page.evaluate(function() 
        {
            var sizeList = document.getElementById('pdpBuyingPanel_SizeChart').children;
            var sizePrices = [];
            for (var i = 0; i < sizeList.length; i++)
            {
                var sizeString;
                var priceString;
                var stockString;
                var anchor = sizeList[i].getElementsByTagName('a');
                if (anchor != null)
                {
                    var sizeButton = anchor[0];
                    sizeString = sizeButton.textContent; 
                    sizeButton.click();
                    var price = document.getElementsByClassName('pdpAddToBagPrice')[0];
                    priceString = price.textContent;
                    var stock = document.getElementsByClassName('pdpStockAvailability')[0].children;
                    for (var j = 0; j < stock.length; j++)
                    {
                        var classname = stock[j].className;
                        if (classname.indexOf('ng-hide-remove') != -1)
                        {
                            stockString = stock[j].textContent;
                        }
                    }
                }
                sizePrices.push({ size : sizeString, price : priceString, stock : stockString });                    
            }
            return sizePrices;
        });
            
        console.log('---START OF PAGE---');
        console.log(page.content);
        console.log('---END OF PAGE---');
        console.log('---START OF PRICEINFO---');
        for (var i = 0; i < sizePrices.length; i++)
        {
            console.log(sizePrices[i].size + ',' + sizePrices[i].price + ',' + sizePrices[i].stock);
        }
        console.log('---END OF PRICEINFO---');
    }
    phantom.exit();
});