import styles from './BackButton.module.css';

    interface BackButtonProps {
    onClick: () => void;
    label?: string;
    }

    export default function BackButton({ onClick, label = 'Назад' }: BackButtonProps) {
    return (
        <button className={styles.btn} onClick={onClick}>
        <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
            <path d="M10 3L5 8L10 13" stroke="currentColor" strokeWidth="1.3" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
        {label}
        </button>
    );
    }

  
