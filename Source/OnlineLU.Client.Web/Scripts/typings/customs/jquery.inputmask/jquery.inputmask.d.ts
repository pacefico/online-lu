interface InputMask {
    (options: any): JQuery;
    (mask: string, events: any): JQuery;
    (method: string): boolean;
}

interface JQuery {

    inputmask: InputMask;
}