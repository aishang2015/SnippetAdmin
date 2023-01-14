import './loading.css';

export function Loading() {

    return (
        <div style={{ width: '100%', height: '100%' }}>
            <div className="loadingspinner">
                <div id="square1"></div>
                <div id="square2"></div>
                <div id="square3"></div>
                <div id="square4"></div>
                <div id="square5"></div>
            </div>
        </div>
    );
}