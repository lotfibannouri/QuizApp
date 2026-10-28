// Le div entoure le MudTextField : on cherche le textarea qu'il contient.
export function getSelectionRange(container) {
    const field = container?.querySelector("textarea, input");
    if (!field) return null;

    const start = field.selectionStart;
    const end = field.selectionEnd;
    return { start, end, text: field.value.substring(start, end) };
}

export function insertText(container) {
    const el = container.querySelector('input, textarea');
    if (!el) return;

    el.focus();
    // Remplace la sélection (ou insère au curseur) et place le curseur après le texte
    el.setRangeText("[" + el.value.substring(el.selectionStart, el.selectionEnd) +"]", el.selectionStart, el.selectionEnd, 'end');
    // Prévenir Blazor que la valeur a changé, pour mettre à jour @bind-Value
    el.dispatchEvent(new Event('input', { bubbles: true }));
    el.dispatchEvent(new Event('change', { bubbles: true }));
}