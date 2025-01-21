function downloadAsSVG(svgElement, filename) {
    const serializer = new XMLSerializer();
    const svgString = serializer.serializeToString(svgElement);
    const blob = new Blob([svgString], { type: "image/svg+xml" });
    const url = URL.createObjectURL(blob);
    
    const link = document.createElement("a");
    link.href = url;
    link.download = filename;
    link.click();
    URL.revokeObjectURL(url);
}

function downloadAsPNG(svgElement, filename) {
    const serializer = new XMLSerializer();
    const svgString = serializer.serializeToString(svgElement);
    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");
    const img = new Image();
    
    img.onload = function () {
        canvas.width = img.width;
        canvas.height = img.height;
        ctx.drawImage(img, 0, 0);
        
        const link = document.createElement("a");
        link.href = canvas.toDataURL("image/png");
        link.download = filename.replace(".svg", ".png");
        link.click();
    };
    
    img.src = "data:image/svg+xml;base64," + btoa(svgString);
}