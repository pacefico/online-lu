interface JQueryBrowser {
    msie: boolean;
    webkit: boolean;
    mozilla: boolean;
    chrome: boolean;
    version: number;
}

interface JQueryStatic {
    browser: JQueryBrowser;
}