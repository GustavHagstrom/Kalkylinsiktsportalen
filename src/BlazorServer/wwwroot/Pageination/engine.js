function pageinate() {
    const root = document.getElementById("print-root");
    const headerBlocks = root.getElementsByClassName("header-block");
    const footerBlocks = root.getElementsByClassName("footer-block");
    const bodyBlocks = root.getElementsByClassName("body-block");
    const PX_PER_MM = 96 / 25.4; // ≈ 3.7795
    const A4_HEIGHT_PX = 297 * PX_PER_MM;


    let headerHeight = 0;
    Array.from(headerBlocks).forEach(block => {
        headerHeight += block.getBoundingClientRect().height;
    });
    let footerHeight = 0;
    Array.from(footerBlocks).forEach(block => {
        footerHeight += block.getBoundingClientRect().height;
    });
    const availableHeight = A4_HEIGHT_PX - headerHeight - footerHeight;
    console.log("availableHeight: " + availableHeight);

    

    //dictionary, key is page number, value is array of body blocks
    const map = new Map();
    let currentBodyHeight = 0;
    let currentPage = 1;
    let warnings = [];

    Array.from(bodyBlocks).forEach(block => {
        currentBodyHeight += block.getBoundingClientRect().height;
        if (block.getBoundingClientRect().height > availableHeight) {
            warnings.push("Block ID: " + block.dataset.id + " is too tall to fit on a single page.");
        }
        if (currentBodyHeight > availableHeight) {
            currentPage += 1;
            currentBodyHeight = block.getBoundingClientRect().height;
        }
        let blockId = block.dataset.id;
        console.log("Block ID: " + blockId + " goes to page " + currentPage);
        console.log(block);
        console.log(block.dataset.pagebreak);

        addToMap(map, currentPage, blockId);

        let pageBreaks = block.querySelectorAll("[data-pagebreak]");
        console.log(pageBreaks);
       
        if (pageBreaks.length > 0) {
            currentPage += 1;
            currentBodyHeight = 0;
        }
    });
    
    const json = Object.fromEntries(map);
    console.log(map);
    console.log(json);

    return json;    
}


function addToMap(map, key, value) {
    if (!map.has(key)) {
        map.set(key, []);
    }

    map.get(key).push(value);
}
