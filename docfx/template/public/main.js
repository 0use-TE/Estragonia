export default {
	defaultTheme: 'dark',
	start() {
		const base = document.documentElement.dataset.baseUrl || '';
		const link = document.createElement('link');
		link.rel = 'stylesheet';
		link.href = `${base}dk-switcher.css`;
		document.head.appendChild(link);

		const script = document.createElement('script');
		script.src = `${base}dk-switcher.js`;
		script.defer = true;
		document.body.appendChild(script);
	}
};
