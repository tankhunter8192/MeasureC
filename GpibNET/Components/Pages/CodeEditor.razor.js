// Neu strukturiert als ES6-Modul
let editor;

export function initialize(elementId) {
    const textArea = document.getElementById(elementId);
    editor = CodeMirror.fromTextArea(textArea, {
        lineNumbers: true,
        mode: "javascript",
        theme: "material",
        autoCloseBrackets: true,
        matchBrackets: true,
        autoCloseTags: true,
        extraKeys: {
            "Ctrl-Space": "autocomplete",
            "Ctrl-Q": function (cm) {
                cm.foldCode(cm.getCursor());
            }
        }
    });
}

export function getContent() {
    return editor?.getValue() || '';
}

export function setContent(content) {
    editor?.setValue(content);
}

export function dispose() {
    editor?.toTextArea();
    editor = null;
}