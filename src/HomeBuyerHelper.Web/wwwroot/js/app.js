// Small JS interop helpers that don't warrant a NuGet package.

// Triggers a browser "Save As" download of `text` as `fileName`, without
// ever sending the data anywhere -- everything stays local to the tab.
window.hbhDownload = (fileName, text) => {
    const blob = new Blob([text], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = fileName;
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
    URL.revokeObjectURL(url);
};
