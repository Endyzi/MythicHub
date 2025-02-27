import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const SearchBar = () => {
    const [searchTerm, setSearchTerm] = useState("");
    const [suggestions, setSuggestions] = useState([]);
    const navigate = useNavigate();

    const handleSearch = async (query) => {
        if (query.length < 3) {
            setSuggestions([]); // clear suggesstion if input is to short
            return;
        }

        try {
            const response = await fetch(`http://localhost:5270/api/blizzard/character/eu/kazzak/${query}`);
            if (response.ok) {
                const data = await response.json();
                setSuggestions([data]); // only one character returns
            } else {
                setSuggestions([]); // clear if no character was found
            }
        } catch (error) {
            console.error("Error fetching character:", error);
            setSuggestions([]);
        }
    };

    const handleSelectCharacter = (character) => {
        navigate(`/character/eu/kazzak/${character.name}`);
        setSearchTerm(""); // Clear search
        setSuggestions([]); // clear the list with suggestions
    };

    return (
        <div className="relative">
            <input
                type="text"
                className="w-full p-2 border rounded"
                placeholder="Search character..."
                value={searchTerm}
                onChange={(e) => {
                    setSearchTerm(e.target.value);
                    handleSearch(e.target.value);
                }}
            />
            {suggestions.length > 0 && (
                <ul className="absolute bg-white border rounded w-full mt-1">
                    {suggestions.map((char) => (
                        <li
                            key={char.id}
                            className="p-2 hover:bg-gray-200 cursor-pointer"
                            onClick={() => handleSelectCharacter(char)}
                        >
                            {char.name} - {char.faction}
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default SearchBar;
