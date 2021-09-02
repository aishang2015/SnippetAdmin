
export function downloadBlob(data: any, fileName: string) {
    const blob = new Blob([data]);
    var downloadElement = document.createElement("a");
    var href = window.URL.createObjectURL(blob);
    downloadElement.href = href;
    downloadElement.download = decodeURIComponent(fileName);
    document.body.appendChild(downloadElement);
    downloadElement.click();
    document.body.removeChild(downloadElement);
    window.URL.revokeObjectURL(href);
}