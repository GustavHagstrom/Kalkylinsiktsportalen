export function measure(root, options = {}) {
    if (!root) {
        throw new Error("Paginated document renderer root is required for measurement.");
    }

    const regionAttribute = options.regionAttribute ?? "data-printable-region";
    const blockIdAttribute = options.blockIdAttribute ?? "data-printable-block-id";
    const pageBreakAttribute = options.pageBreakAttribute ?? "data-page-break-behavior";

    const headerBlocks = getRegionWrappers(root, regionAttribute, "header");
    const footerBlocks = getRegionWrappers(root, regionAttribute, "footer");
    const bodyBlocks = getRegionWrappers(root, regionAttribute, "body");

    return {
        headerHeightPx: sumHeights(headerBlocks),
        footerHeightPx: sumHeights(footerBlocks),
        bodyBlocks: bodyBlocks.map(block => ({
            id: block.getAttribute(blockIdAttribute),
            heightPx: block.getBoundingClientRect().height,
            pageBreakBehavior: block.getAttribute(pageBreakAttribute) ?? "None"
        }))
    };
}

function getRegionWrappers(root, regionAttribute, region) {
    return Array
        .from(root.querySelectorAll(`[${regionAttribute}="${region}"]`))
        .filter(element => root.contains(element));
}

function sumHeights(elements) {
    return elements.reduce(
        (height, element) => height + element.getBoundingClientRect().height,
        0);
}
