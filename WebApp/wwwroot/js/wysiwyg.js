function initWysiwygEditor(editorId, toolbarId, textareaId, content) {
    const textarea = document.querySelector(textareaId);

    const quil = new Quill(editorId, {
        modules: {
            syntax: true,
            toolbar: toolbarId,
        },
        placeholder: 'Type something',
        theme: 'snow'
    })

    if (content) {
        quil.root.innerHTML = content;
    }

    quil.on('text-change', () => {
        textarea.value = quil.root.innerHTML;
    })
}