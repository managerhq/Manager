function autoResizeIframe(iframe) {
  const doc = iframe.contentDocument;
  if (!doc || !doc.documentElement) return;
  const root = doc.documentElement;
  const resize = () => {
    const h = Math.max(root.scrollHeight, doc.body ? doc.body.scrollHeight : 0);
    iframe.style.height = (h + 1) + 'px';
  };
  resize();
  new ResizeObserver(resize).observe(root);
}
