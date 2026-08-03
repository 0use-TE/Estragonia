(() => {
	const versions = ['v1.0.0']; // newest first — keep in sync with docs/ folders
	const docPages = new Set([
		'getting-started',
		'hosting',
		'input-and-rendering',
		'ai-prompt',
		'release-notes',
		'introduction'
	]);

	function parsePath() {
		const path = location.pathname.replace(/\\/g, '/');
		const m = path.match(/\/docs\/(v[\d.]+)(\/zh-CN)?\/([^/]+)\.html$/);
		if (!m) return { version: versions[0], lang: 'en', page: 'getting-started', inDocs: false };
		return {
			version: m[1],
			lang: m[2] ? 'zh-CN' : 'en',
			page: m[3],
			inDocs: true
		};
	}

	function navigate(version, lang, page) {
		const stem = docPages.has(page) ? page : 'getting-started';
		const langSeg = lang === 'zh-CN' ? '/zh-CN' : '';
		const base = document.documentElement.dataset.baseUrl || '';
		location.href = `${base}docs/${version}${langSeg}/${stem}.html`;
	}

	function mount() {
		const nav = document.querySelector('header nav') || document.querySelector('.navbar');
		if (!nav || document.querySelector('.dk-switchers')) return;

		const state = parsePath();
		const box = document.createElement('div');
		box.className = 'dk-switchers';

		const ver = document.createElement('select');
		ver.title = 'Version';
		versions.forEach(v => {
			const o = document.createElement('option');
			o.value = v;
			o.textContent = v;
			if (v === state.version) o.selected = true;
			ver.appendChild(o);
		});

		const lang = document.createElement('select');
		lang.title = 'Lang';
		;[
			['en', 'English'],
			['zh-CN', '简体中文']
		].forEach(([value, label]) => {
			const o = document.createElement('option');
			o.value = value;
			o.textContent = label;
			if (value === state.lang) o.selected = true;
			lang.appendChild(o);
		});

		ver.addEventListener('change', () => navigate(ver.value, lang.value, state.page));
		lang.addEventListener('change', () => navigate(ver.value, lang.value, state.page));

		box.appendChild(ver);
		box.appendChild(lang);
		nav.appendChild(box);
	}

	if (document.readyState === 'loading')
		document.addEventListener('DOMContentLoaded', mount);
	else
		mount();
})();
