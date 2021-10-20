import { ReactElement } from "react";
import { StorageService } from "../../common/storage";


export function RightElement(props: { identify: string, child: ReactElement }) {

    return (
        <>
            {StorageService.getRights().find(r => r === props.identify) &&
                props.child
            }
        </>
    );
}