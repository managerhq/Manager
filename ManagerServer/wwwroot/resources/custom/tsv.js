/**
 * Converts an HTML element (and same-origin iframes within it) into a
 * tab-separated values (TSV) string suitable for pasting into spreadsheets.
 *
 * Approach: walk the DOM and emit text. HTML's natural row/cell delimiters
 * become TSV's row/cell delimiters:
 *   - TD, TH, DD               -> tab (cell separator within a row)
 *   - TR, DT, H1-H6            -> newline (start of a new row)
 *   - block containers (DIV,P, -> newline (so sibling blocks don't run
 *     ADDRESS, HEADER, etc.)      together on one line)
 *   - BR                       -> space (keeps a cell's content on one row)
 *   - SCRIPT, STYLE, NOSCRIPT  -> dropped
 *   - everything else (A, SPAN, STRONG, ...) just contributes text
 *
 * After the walk we normalize whitespace within each cell, strip empty
 * leading/trailing cells per line, and collapse runs of blank lines.
 *
 * Entry point:
 *   elementToTSV(rootElement) -> string
 */

function elementToTSV(root) {
  const ROW_TAGS = new Set([
    "TR", "DT",
    "H1", "H2", "H3", "H4", "H5", "H6",
    "P", "DIV", "LI", "DL", "UL", "OL",
    "ADDRESS", "SECTION", "HEADER", "FOOTER", "ARTICLE", "ASIDE", "NAV",
    "MAIN", "BLOCKQUOTE", "PRE", "HR", "FIGURE", "FIGCAPTION",
    "FORM", "FIELDSET"
  ]);
  const CELL_TAGS = new Set(["TD", "TH", "DD"]);
  const DROP_TAGS = new Set(["SCRIPT", "STYLE", "NOSCRIPT"]);

  const parts = [];

  function walk(node) {
    if (node.nodeType === 3) {
      parts.push(node.nodeValue);
      return;
    }
    if (node.nodeType !== 1) return;

    const tag = node.tagName;
    if (DROP_TAGS.has(tag)) return;
    if (tag === "BR") { parts.push(" "); return; }

    if (tag === "IFRAME") {
      try {
        const doc = node.contentDocument || (node.contentWindow && node.contentWindow.document);
        if (doc && doc.body) walk(doc.body);
      } catch (e) {
        // cross-origin: skip silently
      }
      return;
    }

    if (ROW_TAGS.has(tag)) parts.push("\n");
    else if (CELL_TAGS.has(tag)) parts.push("\t");

    for (const child of node.childNodes) walk(child);
  }

  walk(root);

  const lines = parts.join("").split("\n").map(line => {
    // Every TD/TH/DD emits a leading tab, including the first cell of the
    // row, so strip exactly one leading tab. Do NOT strip more — genuinely
    // empty cells like <th></th><th>X</th> must remain in the output.
    if (line.startsWith("\t")) line = line.slice(1);
    const cells = line.split("\t").map(c => c.replace(/\s+/g, " ").trim());
    if (cells.every(c => c === "")) return "";
    return cells.join("\t");
  });

  const out = [];
  let blank = false;
  for (const line of lines) {
    if (line === "") {
      if (!blank && out.length) out.push("");
      blank = true;
    } else {
      out.push(line);
      blank = false;
    }
  }
  while (out.length && out[out.length - 1] === "") out.pop();

  return out.join("\n");
}
