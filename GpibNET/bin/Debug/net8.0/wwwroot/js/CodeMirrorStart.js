//wwwroot/js/CodeMirrorStart.js
let editor;

export function initializeCodeMirror(elementId) {
    var textArea = document.getElementById(elementId);
    if (textArea) {
        window.codeMirrorEditor = CodeMirror.fromTextArea(textArea, {
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
}

export function getCodeMirrorContent(elementId) {
    if (window.codeMirrorEditor) {
        return window.codeMirrorEditor.getValue();
    }
    return '';
}

export function setCodeMirrorContent(elementId, content) {
    if (window.codeMirrorEditor) {
        window.codeMirrorEditor.setValue(content);
    }
}