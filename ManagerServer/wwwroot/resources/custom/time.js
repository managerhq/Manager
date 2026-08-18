function formatDate(dt, fmt) {
  const d = (dt instanceof Date) ? dt : new Date(dt);
  const H24 = d.getHours(), h12 = H24 % 12 || 12, tt = H24 < 12 ? 'AM' : 'PM';
  const parts = {
    yyyy: String(d.getFullYear()).padStart(4,'0'),
    yy: String(d.getFullYear() % 100).padStart(2,'0'),
    MM: String(d.getMonth()+1).padStart(2,'0'),
    M: String(d.getMonth()+1),
    dd: String(d.getDate()).padStart(2,'0'),
    d: String(d.getDate()),
    HH: String(H24).padStart(2,'0'),
    H: String(H24),
    hh: String(h12).padStart(2,'0'),
    h: String(h12),
    mm: String(d.getMinutes()).padStart(2,'0'),
    m: String(d.getMinutes()),
    ss: String(d.getSeconds()).padStart(2,'0'),
    s: String(d.getSeconds()),
    tt
  };
  const tokens = ["yyyy","HH","hh","MM","dd","mm","ss","tt","yy","H","h","M","d","m","s"]; // longest first
  let out = "", i = 0, n = fmt.length;
  while (i < n) {
    if (fmt[i] === "'") { // quoted literal
      let j = i + 1, lit = "";
      while (j < n) {
        if (fmt[j] === "'" && fmt[j+1] === "'") { lit += "'"; j += 2; }
        else if (fmt[j] === "'") { j++; break; }
        else { lit += fmt[j++]; }
      }
      out += lit; i = j; continue;
    }
    let matched = false;
    for (const t of tokens) {
      if (fmt.startsWith(t, i)) { out += parts[t]; i += t.length; matched = true; break; }
    }
    if (!matched) { out += fmt[i]; i++; } // punctuation, spaces, etc.
  }
  return out;
}

function updateAllTimeElements(format) {
  function updateTimeElements(doc) {
    doc.querySelectorAll('time[datetime]').forEach(el => {
      el.textContent = formatDate(
        new Date(el.getAttribute('datetime')), format);
    });
  }

  // main document
  updateTimeElements(document);

  // handle iframes
  document.querySelectorAll('iframe').forEach(frame => {
    const applyUpdate = () => {
      try {
        if (frame.contentDocument) {
          updateTimeElements(frame.contentDocument);
        }
      } catch (e) {
        // Skipping cross-origin iframe
      }
    };

    frame.addEventListener('load', applyUpdate, { once: true });
    applyUpdate();
  });
}