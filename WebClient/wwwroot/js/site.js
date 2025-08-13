window.site = {
    highlightAll: function () {
        if (window.Prism) window.Prism.highlightAll();
    },
    downloadBytes: function (filename, mimeType, bytes) {
        const blob = new Blob([new Uint8Array(bytes)], { type: mimeType });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = filename || 'download.txt';
        document.body.appendChild(link);
        link.click();
        link.remove();
        setTimeout(() => URL.revokeObjectURL(link.href), 0);
    },
    lsSet: function (key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    },
    lsGet: function (key) {
        const v = localStorage.getItem(key);
        return v ? JSON.parse(v) : null;
    }
};