(() => {
    const container = document.querySelector('.header-glitch-container');
    if (!container) return;

    function generateStaticPattern() {
        const canvas = document.createElement('canvas');
        canvas.width = 100;
        canvas.height = 100;
        const ctx = canvas.getContext('2d');
        const imageData = ctx.createImageData(100, 100);

        for (let i = 0; i < imageData.data.length; i += 4) {
            const noise = Math.random() * 255;
            imageData.data[i] = noise;
            imageData.data[i + 1] = noise;
            imageData.data[i + 2] = noise;
            imageData.data[i + 3] = 255;
        }

        ctx.putImageData(imageData, 0, 0);
        return canvas.toDataURL();
    }

    const staticPattern = generateStaticPattern();

    function createSliceGlitch() {
        const slice = document.createElement('div');
        slice.className = 'glitch-slice';

        const height = 2 + Math.random() * 6;
        const top = Math.random() * container.offsetHeight;

        slice.style.height = `${height}px`;
        slice.style.top = `${top}px`;
        slice.style.animation = `glitch-slice-anim ${200 + Math.random() * 300}ms ease-out`;

        container.appendChild(slice);

        setTimeout(() => slice.remove(), 500);
    }

    function createRGBGlitch() {
        const width = 40 + Math.random() * 100;
        const height = 20 + Math.random() * 60;
        const top = Math.random() * (container.offsetHeight - height);
        const left = Math.random() * (container.offsetWidth - width);

        const red = document.createElement('div');
        red.className = 'glitch-rgb glitch-rgb-red';
        red.style.width = `${width}px`;
        red.style.height = `${height}px`;
        red.style.top = `${top}px`;
        red.style.left = `${left - 3}px`;
        red.style.animation = `glitch-rgb-anim ${400 + Math.random() * 400}ms ease-in-out`;

        const cyan = document.createElement('div');
        cyan.className = 'glitch-rgb glitch-rgb-cyan';
        cyan.style.width = `${width}px`;
        cyan.style.height = `${height}px`;
        cyan.style.top = `${top}px`;
        cyan.style.left = `${left + 3}px`;
        cyan.style.animation = `glitch-rgb-anim ${400 + Math.random() * 400}ms ease-in-out`;

        container.appendChild(red);
        container.appendChild(cyan);

        setTimeout(() => {
            red.remove();
            cyan.remove();
        }, 1000);
    }

    function createStaticGlitch() {
        const block = document.createElement('div');
        block.className = 'glitch-static';

        const width = 60 + Math.random() * 120;
        const height = 40 + Math.random() * 80;
        const top = Math.random() * (container.offsetHeight - height);
        const left = Math.random() * (container.offsetWidth - width);

        block.style.width = `${width}px`;
        block.style.height = `${height}px`;
        block.style.top = `${top}px`;
        block.style.left = `${left}px`;
        block.style.backgroundImage = `url(${staticPattern})`;
        block.style.animation = `glitch-static-anim ${500 + Math.random() * 500}ms linear`;

        container.appendChild(block);

        setTimeout(() => block.remove(), 1200);
    }

    function createScanline() {
        const line = document.createElement('div');
        line.className = 'glitch-scanline';
        line.style.animation = `glitch-scanline-anim ${800 + Math.random() * 400}ms linear`;

        container.appendChild(line);

        setTimeout(() => line.remove(), 1500);
    }

    function createFastScanline() {
        const line = document.createElement('div');
        line.className = 'glitch-scanline-fast';
        line.style.animation = `glitch-scanline-fast-anim ${400 + Math.random() * 200}ms linear`;

        container.appendChild(line);

        setTimeout(() => line.remove(), 800);
    }

    function triggerRandomGlitch() {
        const rand = Math.random();

        if (rand < 0.35) {
            createSliceGlitch();
            if (Math.random() > 0.6) {
                setTimeout(createSliceGlitch, 50 + Math.random() * 100);
            }
        } else if (rand < 0.6) {
            createRGBGlitch();
        } else if (rand < 0.8) {
            createStaticGlitch();
        } else if (rand < 0.9) {
            createFastScanline();
        } else {
            createScanline();
        }
    }

    function startGlitchLoop() {
        triggerRandomGlitch();
        const nextInterval = 250 + Math.random() * 450;
        setTimeout(startGlitchLoop, nextInterval);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            setTimeout(startGlitchLoop, 500);
        });
    } else {
        setTimeout(startGlitchLoop, 500);
    }
})();
