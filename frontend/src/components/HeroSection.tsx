// ============================================
// CARSTAR — HeroSection
// Homepage hero with random car image
// ============================================
import { useMemo } from 'react';
import styles from './HeroSection.module.css';

// Unsplash авто-фото — при завантаженні рандомно обирається одне
// Можеш замінити на власні URL або відносні шляхи до /public
const CAR_IMAGES = [
  'https://i.pinimg.com/1200x/29/9b/77/299b77811b8fa841df7c9a4a2d23fb3a.jpg', // чорне купе
  'https://i.pinimg.com/736x/25/dd/80/25dd80338896e1a7a1d8cdb7e6d6560e.jpg', // червоний спорткар
  'https://i.pinimg.com/736x/5d/4a/18/5d4a18ad8f11281ba24cc169e13a2464.jpg', // чорний BMW
  'https://i.pinimg.com/736x/af/3c/33/af3c33d37377b2279735febb807b19c9.jpg', // інтер'єр
  'https://i.pinimg.com/736x/98/f1/00/98f100fb2c947cd163dfb846b3c16e48.jpg', // сіре авто
  'https://i.pinimg.com/736x/7d/6b/d7/7d6bd741219d325349b19dd7e4af5a85.jpg',
  'https://i.pinimg.com/736x/46/a6/84/46a684e6e72516f3eec0d6773b3e349e.jpg'
];

interface HeroSectionProps {
  onCatalogClick: () => void;
}

export default function HeroSection({ onCatalogClick }: HeroSectionProps) {
  // useMemo — обирається один раз при монтуванні, не змінюється при ре-рендері
  const heroImage = useMemo(
    () => CAR_IMAGES[Math.floor(Math.random() * CAR_IMAGES.length)],
    []
  );

  return (
    <section className={styles.hero}>
      <div className={styles.inner}>
        {/* Left */}
        <div className={styles.left}>
          <div className={styles.eyebrow}>
            <span className={styles.eyebrowLine} />
            Сезон 2025 · Нові надходження
          </div>

          <h1 className={styles.title}>
            Запчастини<br />
            <em>для вашого</em><br />
            автомобіля
          </h1>

          <p className={styles.subtitle}>
            Оригінальні та аналогові запчастини з доставкою
            по Україні. Гарантія якості на кожну позицію.
          </p>

          <div className={styles.btns}>
            <button className={styles.btnPrim} onClick={onCatalogClick}>
              Перейти до каталогу
            </button>
          </div>

          {/* Stats */}
          <div className={styles.stats}>
            <div className={styles.stat}>
              <span className={styles.statVal}>12 400+</span>
              <span className={styles.statLabel}>позицій</span>
            </div>
            <div className={styles.statSep} />
            <div className={styles.stat}>
              <span className={styles.statVal}>98%</span>
              <span className={styles.statLabel}>в наявності</span>
            </div>
            <div className={styles.statSep} />
            <div className={styles.stat}>
              <span className={styles.statVal}>1–2 дні</span>
              <span className={styles.statLabel}>доставка</span>
            </div>
          </div>
        </div>

        {/* Right — car image */}
        <div className={styles.right}>
          <div className={styles.visual}>
            {/* Grid overlay */}
            <div className={styles.visualGrid} />

            {/* Car photo */}
            <img
              src={heroImage}
              alt="Автомобіль"
              className={styles.carImage}
            />

            {/* Gradient overlay — fade знизу */}
            <div className={styles.imageOverlay} />

            {/* Tag — лівий нижній кут */}
            <div className={styles.visualTag}>SS / 25</div>

            {/* Tag — правий верхній кут */}
            <div className={styles.visualTagRight}>CARSTAR</div>
          </div>
        </div>
      </div>

      {/* Bottom marquee */}
      <div className={styles.ticker}>
        <div className={styles.tickerTrack}>
          {['Двигун', 'Гальма', 'Підвіска', 'Фільтри', 'Мастила', 'Кузов', 'Електрика',
            'Акумулятори', 'Оливи', 'Ремені', 'Свічки', 'Радіатори', 'Лампи', 'Датчики'].map((item, i) => (
            <span key={'a' + i} className={styles.tickerItem}>
              {item} <span className={styles.tickerDot}>·</span>
            </span>
          ))}
          {['Двигун', 'Гальма', 'Підвіска', 'Фільтри', 'Мастила', 'Кузов', 'Електрика',
            'Акумулятори', 'Оливи', 'Ремені', 'Свічки', 'Радіатори', 'Лампи', 'Датчики'].map((item, i) => (
            <span key={'b' + i} className={styles.tickerItem} aria-hidden>
              {item} <span className={styles.tickerDot}>·</span>
            </span>
          ))}
        </div>
      </div>
    </section>
  );
}
