function printWindow() {
    window.print();
}

function pageinate() {
    const root = document.getElementById("page-root");
    const headerBlocks = root.getElementsByClassName("header-block");
    const footerBlocks = root.getElementsByClassName("footer-block");
    const bodyBlocks = root.getElementsByClassName("body-block");

    let [page, bodyContainer] = createPage(headerBlocks, footerBlocks);

    Array.from(headerBlocks).forEach(header => headerContainer.append(header));
    Array.from(footerBlocks).forEach(footer => footerContainer.append(footer));


    Array.from(bodyBlocks).forEach(block => {
        bodyContainer.append(block);

        if (page.scrollHeight > page.clientHeight) {
            [page, bodyContainer] = createPage(headerBlocks, footerBlocks);
            bodyContainer.append(block);
        }
    });
}

function createPage(headerBlocks, footerBlocks) {
        const page = createDiv(["page", "A4"]);
        const headerContainer = createDiv(["header"]);
        const footerContainer = createDiv(["footer"]);
        const bodyContainer = createDiv(["body"]);

        page.append(headerContainer);
        page.append(bodyContainer);
        page.append(footerContainer);

        addBlocksToContainer(headerBlocks, headerContainer);
        addBlocksToContainer(footerBlocks, footerContainer);

        return [page, bodyContainer];
}

function createDiv(classList) {
    const div = document.createElement("div");
    classList.forEach(c => div.classList.add(c));
    return div;
}

function addBlocksToContainer(blocks, container) {
    Array.from(blocks).forEach(block => container.append(block));
}
