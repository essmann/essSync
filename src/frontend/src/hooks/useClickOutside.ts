import type { Ref, RefObject } from "react";
import { useEffect } from "react";

export function useClickOutside(ref: RefObject<HTMLElement | null>, callback: () => void) {



    function handleClickOutside(event?: MouseEvent) {
        console.log("You clickd somethging");
        if (ref == null) return;
        if (!ref.current) {
            return;
        }
        if (event?.target instanceof Node) {
            if (!ref.current.contains(event.target)) {
                callback();
            }
        }
    }


    useEffect(() => {
        console.log("Custom hook useeffect just ran");

        let listener = window.addEventListener("mousedown", handleClickOutside);



        //cleanup function, react engine or whatever decides when this is called.

        return () => { window.removeEventListener("mousedown", handleClickOutside) }
    }, [])
}