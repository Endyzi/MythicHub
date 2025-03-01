import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const CharacterProfile = () => {
    const { region, realm, name } = useParams(); // fetching url params
    const [character, setCharacter] = useState(null);

    const fetchCharacter = async () => {
        try {
            const response = await fetch(
                `http://localhost:5270/api/blizzard/character/${region}/${realm}/${name}`
            );
            if (!response.ok) {
                console.error("Character not found!");
                return;
            }
            const data = await response.json();
            console.log("Fetched character data:", data);
            setCharacter(data);
        } catch (error) {
            console.error("Error fetching character:", error);
        }
    };

    useEffect(() => {
        fetchCharacter(); // will run automatically when page is loaded
    }, [region, realm, name]);
    //below values of character. needs to be lowercase to match json response from the blizzard api. Wont work with camel case, unnecessary to convert to lowercase.
    return (
        <div style={{ textAlign: "center", color: "white", padding: "20px" }}>
            {character ? (
                <div style={{ backgroundColor: "#1a2036", borderRadius: "10px", padding: "20px", display: "inline-block", marginTop: "20px" }}>
                    <h2>{character.Title} {character.Name}</h2>
                    <img src={character.CharacterImage} alt={character.name} width="150" />
                    <p><strong>Realm:</strong> {character.realm}</p>
                    <p><strong>Faction:</strong> {character.faction}</p>
                    <p><strong>Race:</strong> {character.race}</p>
                    <p><strong>Class:</strong> {character.characterclass}</p>
                    <p><strong>Spec:</strong> {character.specialization}</p>
                    <p><strong>Level:</strong> {character.level}</p>
                    <p><strong>Item Level:</strong> {character.equippedItemLevel} (Avg: {character.averageItemLevel})</p>
                </div>
            ) : (
                <p>Loading character data...</p>
            )}
        </div>
    );
};

export default CharacterProfile;
