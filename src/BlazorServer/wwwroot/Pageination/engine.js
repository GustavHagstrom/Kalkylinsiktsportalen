function lol() {
    window.print();
}

function pageinate() {
    const root = document.getElementById("print-root");
    const headerBlocks = root.getElementsByClassName("header-block");
    const footerBlocks = root.getElementsByClassName("footer-block");
    const bodyBlocks = root.getElementsByClassName("body-block");

    let [page, bodyContainer] = createPage(headerBlocks, footerBlocks);
    root.append(page);

    console.log(root);
    console.log(page);
    console.log(bodyContainer);
    

    

    Array.from(bodyBlocks).forEach(block => {
        bodyContainer.append(block);

        if (page.scrollHeight > page.clientHeight) {
            [page, bodyContainer] = createPage(headerBlocks, footerBlocks);
            root.append(page);
            bodyContainer.append(block);
            console.log(root);
            console.log(page);
            console.log(bodyContainer);
        }
    });
    console.log("success?");
}

function createPage(headerBlocks, footerBlocks) {
    console.log("skapar ny sida"); 
    const page = createDiv(["page", "A4"]);
    const headerContainer = createDiv(["header"]);
    const footerContainer = createDiv(["footer"]);
    const bodyContainer = createDiv(["body"]);

    page.append(headerContainer);
    page.append(bodyContainer);
    page.append(footerContainer);

    cloneBlockesIntoContainer(headerBlocks, headerContainer);
    cloneBlockesIntoContainer(footerBlocks, footerContainer);

    return [page, bodyContainer];
}

function createDiv(classList) {
    const div = document.createElement("div");
    classList.forEach(c => div.classList.add(c));
    return div;
}

function cloneBlockesIntoContainer(blocks, container) {
    clones = Array.from(blocks).map(block => block.cloneNode(true));
    Array.from(clones).forEach(block => container.append(block));
}
