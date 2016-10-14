declare module ckeditor {
    interface CkeditorOptions {
        skin?: string;
        removePlugins?: string
    }
}

interface JQuery {

    ckeditor(): JQuery;
    ckeditor(callback: () => void): JQuery;
    ckeditor(callback: () => void, options?: ckeditor.CkeditorOptions): JQuery;
    ckeditorGet();
}